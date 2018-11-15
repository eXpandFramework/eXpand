using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.ModelAdapter;


namespace Xpand.ExpressApp.Dashboard.Controllers {
    public interface IModelDashboardModuleDetailViewObjectTypeLink : IModelNode {
        IModelDashboardDetailViewObjectTypeLinkGroups DashboardDetailViewObjectTypeLinkGroups { get; }
    }

    public interface IModelDashboardDetailViewObjectTypeLinkGroups : IModelNode, IModelList<IModelDashboardDetailViewObjectTypeLinkGroup> {
    }

    public interface IModelDashboardDetailViewObjectTypeLinkGroup:IModelNode{
        IModelDashboardDetailViewObjectTypeLinks DashboardDetailViewObjectTypeLinks { get; }
    }

    [ModelNodesGenerator(typeof(DashboardDetailViewObjectTypeLinksNodesGenerator))]
    public interface IModelDashboardDetailViewObjectTypeLinks : IModelNode, IModelList<IModelDashboardDetailViewObjectTypeLink> {

    }

    public interface IModelDashboardDetailViewObjectTypeLink:IModelNodeEnabled{
        [Required]
        [DataSourceProperty("Application.DetailViews")]
        IModelDetailView DetailView { get; set; }
        [DataSourceProperty("Application.BOModel")]
        [Required]
        IModelClass ModelClass { get; set; }
    }

    public class DashboardDetailViewObjectTypeLinksNodesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {

        }
    }

    [ModelAbstractClass]
    public interface IModelListViewMasterDetail : IModelListView {
        [Category(AttributeCategoryNameProvider.Xpand + "." + nameof(DashboardModule))]
        string MasterDetailDashboardView { get; set; }
        [Browsable(false)]
        IEnumerable<IModelDashboardView> DashboardViews { get; }
    }

    [DomainLogic((typeof(IModelListViewMasterDetail)))]
    public class ModelListViewMasterDetailDomainLogic {
        public static IEnumerable<IModelDashboardView> Get_DashboardViews(IModelListViewMasterDetail view) {
            return view.Application.Views.OfType<IModelDashboardView>();
        }

    }

    [ModelAbstractClass]
    public interface IModelDashoardViewMasterDetail : IModelDashboardView {
        [Category(AttributeCategoryNameProvider.Xpand+"."+nameof(DashboardModule) )]
        [ModelBrowsable(typeof(ModelDashboardViewMasterDetailVisibilityCalculator))]
        bool MasterDetail { get; set; }
        [Category(AttributeCategoryNameProvider.Xpand+"."+nameof(DashboardModule) )]
        [ModelBrowsable(typeof(ModelDashboardViewMasterDetailVisibilityCalculator))]
        bool AutoCommitB4CurrentObjectChanged { get; set; }
        [Category(AttributeCategoryNameProvider.Xpand+"."+nameof(DashboardModule) )]
        [ModelBrowsable(typeof(ModelDashboardViewMasterDetailVisibilityCalculator))]
        IModelDashboardDetailViewObjectTypeLinkGroup DetailViewObjectTypeLinkGroup { get; set; }
    }

    [DomainLogic(typeof(IModelDashoardViewMasterDetail))]
    public class ModelDashboardViewMasterDetailDomainLogic {
        public static bool Get_MasterDetail(IModelDashoardViewMasterDetail dashoardViewMasterDetail) {
            return new ModelDashboardViewMasterDetailVisibilityCalculator().IsVisible(dashoardViewMasterDetail, null);
        }
    }

    public class ModelDashboardViewMasterDetailVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            var dashboardViewItems = ((IModelDashoardViewMasterDetail)node).Items.OfType<IModelDashboardViewItem>().ToArray();
            var modelObjectViews = dashboardViewItems.Select(item => item.View).OfType<IModelObjectView>().ToArray();
            return modelObjectViews.Length == 2 && modelObjectViews.Length == dashboardViewItems.Length &&
                   modelObjectViews.GroupBy(view => view.ModelClass).Count() == 1;
        }
    }

    public class MasterDetailActionsController : ViewController<DetailView> {
        public const string ActiveKey = "OnlyInDashboard";
        public const string ActionEnabledKey = "CurrentObject is not null";
        public SimpleAction MasterDetailSaveAction { get; }
        public MasterDetailActionsController() {
            MasterDetailSaveAction = new SimpleAction(this, "MasterDetailSaveAction", PredefinedCategory.Edit.ToString(),
                (s, e) => { ObjectSpace.CommitChanges(); }) {
                Caption = "Save",
                ImageName = "MenuBar_Save"
            };
            Active[ActiveKey] = false;
            MasterDetailSaveAction.Enabled[ActionEnabledKey] = false;
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (Application.GetPlatform()==Platform.Web)
                Frame.Controllers.Cast<Controller>().First(controller => controller.GetType().Name == "ActionsFastCallbackHandlerController").Active[GetType().FullName] = false;
            View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            UpdateActionState();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
        }

        private void ViewOnCurrentObjectChanged(object sender, EventArgs eventArgs) {
            UpdateActionState();
        }

        private void UpdateActionState() {
            MasterDetailSaveAction.Enabled[ActionEnabledKey] = View.CurrentObject != null;
        }
    }

    public class MasterDetailController : ViewController<DashboardView>, IModelExtender {
        private DashboardViewItem _listViewDashboardViewItem;
        private DashboardViewItem _detailViewdashboardViewItem;

        private void ProcessSelectedItem(Object sender, CustomProcessListViewSelectedItemEventArgs e) {
            var innerArgsCurrentObject = e.InnerArgs.CurrentObject;
            if (innerArgsCurrentObject != null) {
                e.Handled = true;
                ProcessSelectedItem(innerArgsCurrentObject);
            }
        }

        public ListView ListView { get; private set; }

        public DetailView DetailView { get; private set; }

        private void ProcessSelectedItem(object innerArgsCurrentObject){
            var detailViewModel = GetDetailViewModel(innerArgsCurrentObject.GetType());
            if (detailViewModel != DetailView.Model){
                if (DetailView.ObjectSpace != null)
                    DetailView.ObjectSpace.Committed -= DetailViewObjectSpaceCommitted;

                DetailView.Close();
                _detailViewdashboardViewItem.Frame.SetView(null);
                var objectSpace = Application.CreateObjectSpace();

                var currentObject = objectSpace.GetObject(innerArgsCurrentObject);
                DetailView = Application.CreateDetailView(objectSpace, detailViewModel.Id, true, currentObject);
                ConfigureDetailView(DetailView);
                _detailViewdashboardViewItem.Frame.SetView(DetailView, true, Frame);
            }
            else{
                if (((IModelDashoardViewMasterDetail) View.Model).AutoCommitB4CurrentObjectChanged&&DetailView.ObjectSpace.ModifiedObjects.Cast<object>().Any())
                    DetailView.ObjectSpace.CommitChanges();
                DetailView.CurrentObject = DetailView.ObjectSpace.GetObject(innerArgsCurrentObject);
            }
        }

        private IModelDetailView GetDetailViewModel(Type currentObjectType){
            var detailViewModel = DetailView.Model;
            var linkGroup = ((IModelDashoardViewMasterDetail) View.Model).DetailViewObjectTypeLinkGroup;
            if (linkGroup != null){
                var currentObjectModelClass = Application.Model.BOModel.GetClass(currentObjectType);
                var modelDetailView =
                    linkGroup.DashboardDetailViewObjectTypeLinks.Where(link => link.ModelClass == currentObjectModelClass)
                        .Select(link => link.DetailView)
                        .FirstOrDefault();
                if (modelDetailView != null)
                    detailViewModel = modelDetailView;
            }
            return detailViewModel;
        }

        private void DetailViewObjectSpaceCommitted(Object sender, EventArgs e) {
            if (((IObjectSpace)sender).IsDeletedObject(DetailView.CurrentObject)) {
                DetailView.CurrentObject = null;
            }
            ListView.ObjectSpace.Refresh();
        }

        private void ListViewObjectSpaceCommitted(Object sender, EventArgs e) {
            IObjectSpace listViewObjectSpace = ((IObjectSpace)sender);
            if (listViewObjectSpace.IsDeletedObject(listViewObjectSpace.GetObject(DetailView.CurrentObject))) {
                DetailView.CurrentObject = null;
            }
            DetailView.ObjectSpace.Refresh();
        }

        private void ListViewDashboardViewItemControlCreated(Object sender, EventArgs e) {
            DisableListViewFastCallbackHandlerController(_listViewDashboardViewItem.Frame);
            var listViewProcessCurrentObjectController = _listViewDashboardViewItem.Frame.GetController<ListViewProcessCurrentObjectController>();
            listViewProcessCurrentObjectController.CustomProcessSelectedItem -= ProcessSelectedItem;
            listViewProcessCurrentObjectController.CustomProcessSelectedItem += ProcessSelectedItem;
            ListView = (ListView)_listViewDashboardViewItem.InnerView;
            ListView.ObjectSpace.Committed -= ListViewObjectSpaceCommitted;
            ListView.ObjectSpace.Committed += ListViewObjectSpaceCommitted;
            if (Application.GetPlatform()==Platform.Win) {
                ListView.CurrentObjectChanged-=ListViewOnCurrentObjectChanged;
                ListView.CurrentObjectChanged+=ListViewOnCurrentObjectChanged;
            }
        }

        private void ListViewOnCurrentObjectChanged(object sender, EventArgs e) {
            var listView = ((ListView) sender);
            listView.Editor.CallMethod("OnProcessSelectedItem");
        }

        private void DisableListViewFastCallbackHandlerController(Frame frame){
            if (Application.GetPlatform()==Platform.Web){
                frame.Controllers.Cast<Controller>()
                    .First(controller => controller.GetType().Name == "ListViewFastCallbackHandlerController")
                    .Active[GetType().FullName] = false;
                
            }
        }

        private void DetailViewdashboardViewItemControlCreated(Object sender, EventArgs e) {
            var frame = ((IFrameContainer)_detailViewdashboardViewItem).Frame;
            frame.GetController<MasterDetailActionsController>().Active[MasterDetailActionsController.ActiveKey] = true;
            frame.GetController<NewObjectViewController>().ObjectCreating += OnObjectCreating;
            ConfigureDetailView((DetailView)_detailViewdashboardViewItem.InnerView);
        }

        private void OnObjectCreating(object sender, ObjectCreatingEventArgs objectCreatingEventArgs) {
            objectCreatingEventArgs.ShowDetailView = false;
            Application.DetailViewCreated += ApplicationOnDetailViewCreated;
        }

        private void ApplicationOnDetailViewCreated(object sender, DetailViewCreatedEventArgs detailViewCreatedEventArgs) {
            Application.DetailViewCreated -= ApplicationOnDetailViewCreated;
            _detailViewdashboardViewItem.Frame.GetController<MasterDetailActionsController>().MasterDetailSaveAction.Enabled[MasterDetailActionsController.ActionEnabledKey] = true;
            _detailViewdashboardViewItem.ControlCreated -= DetailViewdashboardViewItemControlCreated;
            ConfigureDetailView(detailViewCreatedEventArgs.View);
        }

        private void ConfigureDetailView(DetailView detailView) {
            DetailView = detailView;
            foreach (var listPropertyEditor in detailView.GetItems<ListPropertyEditor>()){
                listPropertyEditor.ControlCreated+=ListPropertyEditorOnControlCreated;

            }
            detailView.ViewEditMode = ViewEditMode.Edit;
            var objectSpace = detailView.ObjectSpace;
            if (objectSpace != null) {
                objectSpace.Committed -= DetailViewObjectSpaceCommitted;
                objectSpace.Committed += DetailViewObjectSpaceCommitted;
            }
        }

        private void ListPropertyEditorOnControlCreated(object sender, EventArgs eventArgs){
            var listPropertyEditor = ((ListPropertyEditor) sender);
            listPropertyEditor.ControlCreated-=ListPropertyEditorOnControlCreated;
            DisableListViewFastCallbackHandlerController(listPropertyEditor.Frame);
        }


        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            if (Frame.Context == TemplateContext.ApplicationWindow) {
                var showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
                showNavigationItemController.ItemsInitialized += ShowNavigationItemControllerOnItemsInitialized;
            }
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (((IModelDashoardViewMasterDetail)View.Model).MasterDetail) {
                _listViewDashboardViewItem = View.GetItems<DashboardViewItem>().First(item => item.Model.View is IModelListView);
                _detailViewdashboardViewItem = View.GetItems<DashboardViewItem>().Except(new[] { _listViewDashboardViewItem }).First();
                _listViewDashboardViewItem.ControlCreated += ListViewDashboardViewItemControlCreated;
                _detailViewdashboardViewItem.ControlCreated += DetailViewdashboardViewItemControlCreated;
            }
        }

        protected override void OnDeactivated() {
            if (_listViewDashboardViewItem != null) {
                _listViewDashboardViewItem.ControlCreated -= ListViewDashboardViewItemControlCreated;
            }
            if (_detailViewdashboardViewItem != null) {
                _detailViewdashboardViewItem.ControlCreated -= DetailViewdashboardViewItemControlCreated;
            }
            if (DetailView?.ObjectSpace != null)
                DetailView.ObjectSpace.Committed -= DetailViewObjectSpaceCommitted;
            if (ListView != null) {
                ListView.ObjectSpace.Committed -= ListViewObjectSpaceCommitted;
            }
            base.OnDeactivated();
        }

        IEnumerable<T> GetItems<T>(IEnumerable collection, Func<T, IEnumerable> selector) {
            var stack = new Stack<IEnumerable<T>>();
            stack.Push(collection.OfType<T>());

            while (stack.Count > 0) {
                IEnumerable<T> items = stack.Pop();
                foreach (var item in items) {
                    yield return item;
                    IEnumerable<T> children = selector(item).OfType<T>();
                    stack.Push(children);
                }
            }
        }

        private void ShowNavigationItemControllerOnItemsInitialized(object sender, EventArgs eventArgs) {
            var showNavigationItemController = ((ShowNavigationItemController)sender);
            var choiceActionItems = GetItems<ChoiceActionItem>(showNavigationItemController.ShowNavigationItemAction.Items,
                    item => item.Items).Where(NotHaveRights).ToArray();
            for (int index = choiceActionItems.Length - 1; index >= 0; index--) {
                var choiceActionItem = choiceActionItems[index];
                choiceActionItem.ParentItem.Items.Remove(choiceActionItem);
            }
        }

        private bool NotHaveRights(ChoiceActionItem choiceActionItem) {
            var viewShortcut = choiceActionItem.Data as ViewShortcut;
            if (viewShortcut != null) {
                var viewId = viewShortcut.ViewId;
                if (Application.Model.Views[viewId] is IModelDashboardView modelDashboardView && ((IModelDashoardViewMasterDetail)modelDashboardView).MasterDetail) {
                    var type = modelDashboardView.Items.OfType<IModelDashboardViewItem>().Select(item => item.View.AsObjectView.ModelClass.TypeInfo.Type).First();
                    return SecuritySystem.CurrentUser != null && !SecuritySystem.IsGranted(ObjectSpace, type, SecurityOperations.ReadOnlyAccess, null, null);
                }
            }
            return false;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelDashboardView, IModelDashoardViewMasterDetail>();
            extenders.Add<IModelListView, IModelListViewMasterDetail>();
            extenders.Add<IModelDashboardModule, IModelDashboardModuleDetailViewObjectTypeLink>();
        }
    }
}
