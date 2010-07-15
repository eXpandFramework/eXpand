using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.Win.ListEditors;
using NewItemRowPosition = DevExpress.ExpressApp.NewItemRowPosition;
using XafGridView = DevExpress.ExpressApp.Win.Editors.XafGridView;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelListViewMasterDetail : IModelNode
    {
        IModelMasterDetail MasterDetail { get; }
    }
    [ReadOnly(true)]
    [ModelNodesGenerator(typeof(ModelMasterDetailNodesGenerator))]
    public interface IModelMasterDetail : IModelNode, IModelList<IModelDetailRelation>
    {
        bool Enable { get; set; }
    }

    public class ModelMasterDetailNodesGenerator : ModelNodesGeneratorBase
    {
        protected override void GenerateNodesCore(ModelNode node)
        {
            var modelListView = ((IModelListView)node.Parent);
            IEnumerable<IModelMember> modelMembers = modelListView.ModelClass.AllMembers.Where(member => member.MemberInfo.IsAssociation && member.MemberInfo.IsList);
            foreach (var modelMember in modelMembers)
            {
                var modelMasterDetail = ((IModelMasterDetail)node);
                if (modelMasterDetail.GetNode<IModelDetailRelation>(modelMember.Name) == null)
                {
                    var modelDetailRelation = modelMasterDetail.AddNode<IModelDetailRelation>(modelMember.Name);
                    modelDetailRelation.CollectionMemberName = modelMember.Name;
                }
            }
        }
    }

    [KeyProperty("CollectionMemberName")]
    [DisplayPropertyAttribute("CollectionMemberName")]
    public interface IModelDetailRelation : IModelNode
    {
        [DataSourceProperty("ListViews")]
        IModelListView ListView { get; set; }

        [Browsable(false)]
        IModelList<IModelListView> ListViews { get; }

        [Category("eXpand")]
        [Description("The collection member that is going to be used as child collection")]
        [ReadOnly(true)]
        string CollectionMemberName { get; set; }
    }
    [DomainLogic(typeof(IModelDetailRelation))]
    public class MasterDetailOptionsDomainLogic
    {
        public static IModelList<IModelListView> Get_ListViews(IModelDetailRelation modelDetailRelation)
        {
            var modelListViews = new CalculatedModelNodeList<IModelListView>();
            var modelListView = (IModelListView) modelDetailRelation.Parent.Parent;
            Type listType = modelListView.ModelClass.FindMember(modelDetailRelation.CollectionMemberName).MemberInfo.ListElementTypeInfo.Type;
            IModelClass modelClass = modelDetailRelation.Application.BOModel.GetClass(listType);
            var listViews = modelListView.Application.Views.OfType<IModelListView>().Where(modelView => modelView.ModelClass==modelClass);
            modelListViews.AddRange(listViews);
            return modelListViews;
        }
    }
    public class GridViewBuilder {

//        public void SetGridViewMasterProperties(IModelListView modelListView, XafGridView xafGridView)
//        {
//            IEnumerable<IModelDetailRelation> modelDetailRelations = ((IModelListViewMasterDetail)modelListView).MasterDetail.Where(relation => relation.ListView != null);
//            xafGridView.OptionsDetail.ShowDetailTabs = modelDetailRelations.Count() > 1;
//            xafGridView.OptionsView.ShowDetailButtons = modelDetailRelations.Count() > 0;
//            xafGridView.OptionsDetail.EnableMasterViewMode = ((IModelListViewMasterDetail)modelListView).MasterDetail.Enable;
//        }

//        public ListEditors.XafGridView GetDefaultXafGridView(XafGridView masterGridView, int rowHandle, int relationIndex, IModelListView childModelListView,List<Controller> controllers,ListView listView)
//        {
//            return GetDefaultXafGridView(childModelListView, masterGridView.GridControl, controllers,listView);
//        }

//        ListEditors.XafGridView GetDefaultXafGridView(IModelListView modelListView, GridControl gridControl, List<Controller> controllers,ListView listView)
//        {
//            Type type = modelListView.ModelClass.TypeInfo.Type;
//            CollectionSourceBase collectionSourceBase = _xafApplication.CreateCollectionSource(_objectSpace.CreateNestedObjectSpace(), type, modelListView.Id);
//            ListView listView = _xafApplication.CreateListView(modelListView, collectionSourceBase, true);
//            listView.CreateControls();
//            var baseView = (ListEditors.XafGridView)(((GridControl)listView.Control)).MainView;
//            SetGridViewMasterProperties(modelListView, baseView);
//            baseView.OptionsView.ShowDetailButtons = true;
//            baseView.OptionsDetail.EnableMasterViewMode = true;
//            gridControl.ViewCollection.Add(baseView);
//            var nestedControllersManager = new NestedControllersManager(_xafApplication);
//            controllers.AddRange(nestedControllersManager.GetControllers(listView));
//            Window window = _xafApplication.CreateWindow(TemplateContext.View, controllers, false, true);
//            window.SetView(listView);
//            return baseView;
//        }

        public XafGridView GetLevelDefaultView(XafGridView masterGridView, int rowHandle, int relationIndex, IModelListView modelListView,XafApplication xafApplication, ObjectSpace _objectSpace)
        {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(modelListView, masterGridView);
            bool isRelationSet = modelDetailRelationCalculator.IsRelationSet(rowHandle, relationIndex);
            if (isRelationSet)
            {
                IModelListView childModelListView = modelDetailRelationCalculator.GetChildModelListView(rowHandle, relationIndex);

                Window window = xafApplication.CreateWindow(TemplateContext.View, null, true, true);
                var listViewBuilder = new ListViewBuilder(xafApplication, _objectSpace);
                ListView listView = listViewBuilder.CreateListView(childModelListView);
                ListEditors.XafGridView defaultXafGridView = null;
                EventHandler[] listViewOnControlsCreated = { null };
                listViewOnControlsCreated[0] = (sender, args) =>
                {
                    defaultXafGridView = (ListEditors.XafGridView)((GridListEditor)((ListView)sender).Editor).GridView;
                    defaultXafGridView.OwnerPropertyName = childModelListView.Id;
                    defaultXafGridView.Window = window;
                    defaultXafGridView.GridControl = masterGridView.GridControl;
                    listView.ControlsCreated -= listViewOnControlsCreated[0];
                };
                listView.ControlsCreated += listViewOnControlsCreated[0];
                window.SetView(listView);
                return defaultXafGridView;
            }
            return null;
        }

    }

    public class ListViewBuilder {
        readonly XafApplication _xafApplication;
        readonly ObjectSpace _objectSpace;
        GridListEditor _gridListEditor;
        XafGridView _xafGridView;

        public ListViewBuilder(XafApplication xafApplication,ObjectSpace objectSpace) {
            _xafApplication = xafApplication;
            _objectSpace = objectSpace;
        }

        public ListView CreateListView(IModelListView modelListView)
        {
            Type type = modelListView.ModelClass.TypeInfo.Type;
            CollectionSourceBase collectionSourceBase = _xafApplication.CreateCollectionSource(_objectSpace.CreateNestedObjectSpace(), type, modelListView.Id);
            ListView listView = _xafApplication.CreateListView(modelListView, collectionSourceBase, true);
//            listView.CreateControls();
            return listView;
        }

        public ListView CreateListView(IModelListView modelListView, XafGridView xafGridView) {
            _xafGridView = xafGridView;
            Type type = modelListView.ModelClass.TypeInfo.Type;
            CollectionSourceBase collectionSourceBase = _xafApplication.CreateCollectionSource(_objectSpace.CreateNestedObjectSpace(), type, modelListView.Id);
            _gridListEditor = new GridListEditor(modelListView);
            _gridListEditor.CustomGridViewCreate+=GridListEditorOnCustomGridViewCreate;
            _gridListEditor.CustomGridCreate+=GridListEditorOnCustomGridCreate;
            
            
            
            ((ISupportCustomListEditorCreation) _xafApplication).CustomCreateListEditor +=OnCustomCreateListEditor;
            var listView = _xafApplication.CreateListView(modelListView, collectionSourceBase, false);
            listView.CreateControls();


            return listView;
        }

        void GridListEditorOnCustomGridCreate(object sender, CustomGridCreateEventArgs customGridCreateEventArgs) {
            _gridListEditor.CustomGridCreate -= GridListEditorOnCustomGridCreate;
//            customGridCreateEventArgs.Handled = true;
            customGridCreateEventArgs.Grid = _xafGridView.GridControl;
        }

        void GridListEditorOnCustomGridViewCreate(object sender, CustomGridViewCreateEventArgs args) {
            _gridListEditor.CustomGridViewCreate -= GridListEditorOnCustomGridViewCreate;
            args.GridView = (ListEditors.XafGridView) _xafGridView;
//            args.Handled = true;
        }

        void OnCustomCreateListEditor(object sender, CreatingListEditorEventArgs args) {
            ((ISupportCustomListEditorCreation) _xafApplication).CustomCreateListEditor-=OnCustomCreateListEditor;
            args.Handled = true;
            args.ListEditor = _gridListEditor;
        }

    }
    public class NestedControllersManager {
        readonly XafApplication _xafApplication;

        public NestedControllersManager(XafApplication xafApplication) {
            _xafApplication = xafApplication;
        }
        NewItemRowPosition GetNewItemRowPosition(NewItemRowListViewController newItemRowListViewController, NewObjectViewController newObjectViewController)
        {
            newObjectViewController.NewObjectAction.Active.Clear();
            newObjectViewController.NewObjectAction.Enabled.Clear();
            NewItemRowPosition newItemRowPosition = NewItemRowPosition.None;
            newItemRowListViewController.CustomCalculateNewItemRowPosition += (sender, args) => newItemRowPosition = args.NewItemRowPosition;
            return newItemRowPosition;
        }

        public IEnumerable<Controller> GetControllers(ListView listView) {
            return GetNewItemRowPositionControllers(listView);
        }
        IEnumerable<Controller> GetNewItemRowPositionControllers(ListView listView)
        {
            var newItemRowListViewController = _xafApplication.CreateController<NewItemRowListViewController>();
            var newObjectViewController = _xafApplication.CreateController<NewObjectViewController>();
            ((ISupportNewItemRowPosition)listView.Editor).NewItemRowPosition = GetNewItemRowPosition(newItemRowListViewController, newObjectViewController);
            return new List<Controller> { newItemRowListViewController, newObjectViewController };
        }

    }
    public class ModelDetailRelationCalculator
    {
        readonly IModelListView _modelListView;
        readonly XafGridView _xafGridView;

        public ModelDetailRelationCalculator(IModelListView modelListView,XafGridView xafGridView) {
            _modelListView = modelListView;
            _xafGridView = xafGridView;
        }

        public  bool IsRelationSet( int rowHandle, int relationIndex)
        {
            string rName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            IModelDetailRelation modelDetailRelation = ((IModelListViewMasterDetail)_modelListView).MasterDetail.Where(relation => relation.CollectionMemberName == rName && relation.ListView != null).FirstOrDefault();
            return modelDetailRelation!= null;
        }

        public IModelListView GetChildModelListView(int rowHandle, int relationIndex)
        {
            var rName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            return ((IModelListViewMasterDetail)_modelListView).MasterDetail.Where(relation => relation.CollectionMemberName == rName).Single().ListView;
        }

        public string GetOwnerPropertyName(IModelListView childModelListView,int rowHandle, int relationIndex) {
            var rName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            IMemberInfo associatedMemberInfo = _modelListView.ModelClass.AllMembers.Where(member => member.MemberInfo.Name == rName).Single().MemberInfo.AssociatedMemberInfo;
            return associatedMemberInfo.Name;

        }
    }
    struct LevelDefaultViewInfoCreation
    {
        public LevelDefaultViewInfoCreation(int rowHandle, int relationIndex)
            : this()
        {
            RowHandle = rowHandle;
            RelationIndex = relationIndex;
        }

        public int RowHandle { get; set; }
        public int RelationIndex { get; set; }
    } 

    public class MasterDetailViewController : ViewController<ListView>,IModelExtender
    {
        
        Dictionary<LevelDefaultViewInfoCreation, bool> _levelDefaultViewCreation;
        Window _windowToDispose;

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            if (((IModelListViewMasterDetail)View.Model).MasterDetail.Enable)
            {
                _levelDefaultViewCreation = null;
                XafGridView view = ((GridListEditor)View.Editor).GridView;
                view.MasterRowExpanded -= view_MasterRowExpanded;
                view.MasterRowCollapsed-=ViewOnMasterRowCollapsed;
                view.MasterRowCollapsing-=ViewOnMasterRowCollapsing;
                view.MasterRowEmpty -= ViewOnMasterRowEmpty;
                view.MasterRowGetLevelDefaultView -= ViewOnMasterRowGetLevelDefaultView;
            }
        }

        void ViewOnMasterRowCollapsing(object sender, MasterRowCanExpandEventArgs masterRowCanExpandEventArgs) {
            _windowToDispose = ((ListEditors.XafGridView)((GridView) sender).GetDetailView(masterRowCanExpandEventArgs.RowHandle,masterRowCanExpandEventArgs.RelationIndex)).Window;
        }

        void ViewOnMasterRowCollapsed(object sender, CustomMasterRowEventArgs customMasterRowEventArgs) {
            ((WinWindow) _windowToDispose).Form.Close();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (((IModelListViewMasterDetail)View.Model).MasterDetail.Enable) {
                _levelDefaultViewCreation = new Dictionary<LevelDefaultViewInfoCreation, bool>();
                XafGridView view = ((GridListEditor)View.Editor).GridView;
                view.MasterRowCollapsed += ViewOnMasterRowCollapsed;
                view.MasterRowExpanded += view_MasterRowExpanded;
                view.MasterRowCollapsing += ViewOnMasterRowCollapsing;
                view.MasterRowEmpty += ViewOnMasterRowEmpty;
                view.MasterRowGetLevelDefaultView += ViewOnMasterRowGetLevelDefaultView;
            }
        }


        void view_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            _levelDefaultViewCreation[new LevelDefaultViewInfoCreation(e.RowHandle, e.RelationIndex)] = false;
        }


        void ViewOnMasterRowEmpty(object sender, MasterRowEmptyEventArgs eventArgs) {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(View.Model, (XafGridView) sender);
            eventArgs.IsEmpty=!modelDetailRelationCalculator.IsRelationSet(eventArgs.RowHandle, eventArgs.RelationIndex);
        }

        void ViewOnMasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e) {
            var levelDefaultViewInfoCreation = new LevelDefaultViewInfoCreation(e.RowHandle, e.RelationIndex);
            if (!_levelDefaultViewCreation.ContainsKey(levelDefaultViewInfoCreation) || !(_levelDefaultViewCreation[levelDefaultViewInfoCreation])) {
                if (!_levelDefaultViewCreation.ContainsKey(levelDefaultViewInfoCreation))
                    _levelDefaultViewCreation.Add(levelDefaultViewInfoCreation, true);
                var gridViewBuilder = new GridViewBuilder();
                var levelDefaultView =
                    (ListEditors.XafGridView)
                    gridViewBuilder.GetLevelDefaultView((XafGridView) sender, e.RowHandle, e.RelationIndex, View.Model,
                                                        Application, ObjectSpace);
                e.DefaultView = levelDefaultView;
            }

        }





        void CreateChildObject(XafGridView masterView, CustomMasterRowEventArgs e)
        {
            string relationName = masterView.GetRelationName(e.RowHandle, e.RelationIndex);
            object masterObject = masterView.GetRow(e.RowHandle);
            IMemberInfo memberInfo = XafTypesInfo.Instance.FindTypeInfo(masterObject.GetType()).FindMember(relationName);
            Type listElementType = memberInfo.ListElementType;
            IMemberInfo referenceToOwner = memberInfo.ListElementTypeInfo.ReferenceToOwner;
            object obj = ObjectSpace.CreateObject(listElementType);
            referenceToOwner.SetValue(obj, masterObject);
        }

        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewMasterDetail>();
        }

        #endregion
    }
}
