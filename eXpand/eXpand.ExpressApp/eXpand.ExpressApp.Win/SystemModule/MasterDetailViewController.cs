using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

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
        readonly XafApplication _xafApplication;
        readonly ObjectSpace _objectSpace;

        public GridViewBuilder(XafApplication xafApplication,ObjectSpace objectSpace) {
            _xafApplication = xafApplication;
            _objectSpace = objectSpace;
        }

        public void SetGridViewMasterProperties(IModelListView modelListView, XafGridView xafGridView)
        {
            IEnumerable<IModelDetailRelation> modelDetailRelations = ((IModelListViewMasterDetail)modelListView).MasterDetail.Where(relation => relation.ListView != null);
            xafGridView.OptionsDetail.ShowDetailTabs = modelDetailRelations.Count() > 1;
            xafGridView.OptionsView.ShowDetailButtons = modelDetailRelations.Count() > 0;
            xafGridView.OptionsDetail.EnableMasterViewMode = ((IModelListViewMasterDetail)modelListView).MasterDetail.Enable;
        }

        public XafGridView GetDefaultXafGridView(XafGridView masterGridView, int rowHandle, int relationIndex, IModelListView childModelListView)
        {
            return GetDefaultXafGridView(childModelListView,masterGridView.GridControl);
        }

        XafGridView GetDefaultXafGridView(IModelListView modelListView, GridControl gridControl)
        {
            Type type = modelListView.ModelClass.TypeInfo.Type;
            CollectionSourceBase collectionSourceBase = _xafApplication.CreateCollectionSource(_objectSpace.CreateNestedObjectSpace(), type, modelListView.Id);
            ListView listView = _xafApplication.CreateListView(modelListView, collectionSourceBase, true);
            
            listView.CreateControls();
            var baseView = (XafGridView)(((GridControl)listView.Control)).MainView;
            SetGridViewMasterProperties(modelListView, baseView);
            baseView.OptionsView.ShowDetailButtons = true;
            baseView.OptionsDetail.EnableMasterViewMode = true;
            gridControl.ViewCollection.Add(baseView);
//            baseView.MasterRowGetLevelDefaultView += BaseViewOnMasterRowGetLevelDefaultView;
//            SubscribeToEvents(baseView);
            return baseView;
        }

    }

    public class ModelDetailRelationWrapper
    {
        readonly IModelListView _modelListView;
        readonly XafGridView _xafGridView;

        public ModelDetailRelationWrapper(IModelListView modelListView,XafGridView xafGridView) {
            _modelListView = modelListView;
            _xafGridView = xafGridView;
        }

        public  bool IsRelationSet( int rowHandle, int relationIndex)
        {
            string rName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            int count = ((IModelListViewMasterDetail)_modelListView).MasterDetail.Where(relation => relation.CollectionMemberName == rName && relation.ListView != null).Count();
            return count == 1;
        }

        public IModelListView GetChildModelListView(int rowHandle, int relationIndex)
        {
            var rName = _xafGridView.GetRelationName(rowHandle, relationIndex);
            return ((IModelListViewMasterDetail)_modelListView).MasterDetail.Where(relation => relation.CollectionMemberName == rName).Single().ListView;
        }
    
    }
    public class MasterDetailViewController : ViewController<ListView>,IModelExtender
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            Active["Model"] = ((IModelListViewMasterDetail)View.Model).MasterDetail.Enable;
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var grid = (GridControl)View.Control;
            var view = (XafGridView)grid.FocusedView;
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace);
            gridViewBuilder.SetGridViewMasterProperties(View.Model,view);
            SubscribeToEvents(view,View.Model);
        }


        void SubscribeToEvents(GridView view,IModelListView modelListView) {
            view.MasterRowExpanded += view_MasterRowExpanded;
            view.MasterRowEmpty +=(o, eventArgs) =>
                ViewOnMasterRowEmpty((XafGridView) o, eventArgs.RowHandle, eventArgs.RelationIndex, modelListView);
            view.MasterRowGetLevelDefaultView += (sender, args) => args.DefaultView = GetLevelDefaultView((XafGridView)sender, args.RowHandle, args.RelationIndex, modelListView);
        }



        bool ViewOnMasterRowEmpty(XafGridView xafGridView, int rowHandle,int relationIndex,IModelListView modelListView)
        {
            var modelDetailRelationWrapper = new ModelDetailRelationWrapper(modelListView,xafGridView);
            return modelDetailRelationWrapper.IsRelationSet(rowHandle, relationIndex);
        }


        XafGridView GetLevelDefaultView(XafGridView masterGridView,int rowHandle,int relationIndex,IModelListView modelListView) {
            var modelDetailRelationWrapper = new ModelDetailRelationWrapper(modelListView,masterGridView);
            bool isRelationSet = modelDetailRelationWrapper.IsRelationSet(rowHandle,relationIndex);
            if (!isRelationSet)
                return null;
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace);
            IModelListView childModelListView = modelDetailRelationWrapper.GetChildModelListView(rowHandle,relationIndex);
            XafGridView defaultXafGridView = gridViewBuilder.GetDefaultXafGridView(masterGridView, rowHandle, relationIndex,  childModelListView);
            gridViewBuilder.SetGridViewMasterProperties(childModelListView, defaultXafGridView);
            SubscribeToEvents(defaultXafGridView, childModelListView);
            return defaultXafGridView;
        }


        void view_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            var masterView = ((XafGridView)sender);
            var detailView = ((XafGridView)masterView.GetDetailView(e.RowHandle, e.RelationIndex));
            ((XafCurrencyDataController)detailView.DataController).NewItemRowObjectCustomAdding += (o, args) =>
            {
                args.Handled = true;
                CreateChildObject(masterView, e);
            };
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
