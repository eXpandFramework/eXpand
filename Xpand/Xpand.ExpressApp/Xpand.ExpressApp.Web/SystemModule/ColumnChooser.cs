using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.TreeNode;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface IModelOptionsColumnChooserContextMenu : IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [ModelReadOnly(typeof(ModelColumnChooserReadOnlyCalculator))]
        [Description("Hierarchical support when XpandTreeListEditorsAspNetModule is installed")]
        bool ColumnChooserContextMenu { get; set; }
    }

    [ModelAbstractClass]
    public interface IModelMemberColumnChooserListView:IModelMember{
        [Category(AttributeCategoryNameProvider.Xpand)]
        [DataSourceProperty("Application.Views")]
        [DataSourceCriteria("(AsObjectView Is Not Null) And (AsObjectView.ModelClass Is Not Null) And ('@This.ModelClass' = AsObjectView.ModelClass)")]
        [ModelBrowsable(typeof(ColumnChooserListViewVisibilityCalulator))]
        [ModelReadOnly(typeof(ModelColumnChooserReadOnlyCalculator))]
        IModelListView ColumnChooserListView { get; set; }
    }

    [DomainLogic(typeof(IModelMemberColumnChooserListView))]
    public class ModelMemberColumnChooserListViewLogic {
        public static IModelListView Get_ColumnChooserListView(IModelMemberColumnChooserListView chooserListView){
            var memberType = chooserListView.ModelClass.TypeInfo.FindMember(chooserListView.Name)?.MemberType;
            return memberType != null ? chooserListView.Application.BOModel.GetClass(memberType)?.DefaultListView : null;
        }
    }

    public interface IModelColumnColumnChooserListView : IModelColumn, IColumnChooserListView {
    }

    [ModelInterfaceImplementor(typeof(IModelOptionsColumnChooserContextMenu), "Application.Options")]
    public interface IModelListViewColumnChooserContextMenu : IModelOptionsColumnChooserContextMenu {

    }
    public class ColumnChooserGridViewController:ViewController<ListView>,IXafCallbackHandler ,IModelExtender{
        public ColumnChooserGridViewController() {
            ColumnChooserAction = new SimpleAction(this, "ColumnChooser", "HiddenCategory");
            ColumnChooserAction.Execute += ColumnChooserActionOnExecute;
        }

        protected override void OnActivated(){
            base.OnActivated();
            Active["XpandTreeModule"]= !new ModelColumnChooserReadOnlyCalculator().IsReadOnly(Application.Model.Options, (IModelNode)null);
        }

        public SimpleAction ColumnChooserAction { get; }
        private void ColumnChooserActionOnExecute(object sender, SimpleActionExecuteEventArgs e) {
            var showViewParameters = e.ShowViewParameters;
            showViewParameters.TargetWindow = TargetWindow.NewWindow;
            var objectSpace = Application.CreateObjectSpace(typeof(ColumnChooserList));
            var detailView = Application.CreateDetailView(objectSpace, new ColumnChooserList());
            detailView.ViewEditMode = ViewEditMode.Edit;
            showViewParameters.CreatedView = detailView;
            var dialogController = Application.CreateController<DialogController>();
            dialogController.CancelAction.Active[GetType().Name] = false;
            showViewParameters.Controllers.Add(dialogController);
            showViewParameters.Controllers.Add(new PopupParentFrameController(Frame));
            dialogController.AcceptAction.Execute += AcceptActionOnExecute;
        }

        private void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            Frame.SetView(Application.ProcessShortcut(View.CreateShortcut()));
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var gridListEditor = View.Editor as ASPxGridListEditor;
            if (gridListEditor != null&& ((IModelListViewColumnChooserContextMenu) View.Model).ColumnChooserContextMenu) {
                XafCallbackManager.RegisterHandler(GetType().FullName, this);
                gridListEditor.Grid.FillContextMenuItems += GridOnFillContextMenuItems;
                var script = XafCallbackManager.GetScript(GetType().FullName, "");
                string check="if (e.item.name == '"+ColumnChooserAction.Id+"') ";
                gridListEditor.Grid.ClientSideEvents.ContextMenuItemClick = $@"function(s, e){{{check}{script}}}";
            }
        }
        XafCallbackManager XafCallbackManager => ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager;

        public void ProcessAction(string parameter){
            ColumnChooserAction.DoExecute();
        }

        private void GridOnFillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e) {
            if (e.MenuType == GridViewContextMenuType.Columns) {
                var item = e.CreateItem(ColumnChooserAction.Caption, ColumnChooserAction.Id);
                ASPxImageHelper.SetImageProperties(item.Image, ColumnChooserAction.ImageName);
                e.Items[10].Visible = false;
                e.Items.Insert(10, item);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions,IModelOptionsColumnChooserContextMenu>();
            extenders.Add<IModelListView,IModelListViewColumnChooserContextMenu>();
            extenders.Add<IModelMember, IModelMemberColumnChooserListView>();
            extenders.Add<IModelColumn,IModelColumnColumnChooserListView>();
        }
    }

}

