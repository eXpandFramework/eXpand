using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.Persistent.Base.General.Model {
    
    public interface IModelApplicationListViews:IModelNode {
        [Browsable(false)]
        IModelList<IModelListView> ListViews { get; }
    }
    [DomainLogic(typeof(IModelApplicationListViews))]
    public class ModelApplicationListViewsDomainLogic {
        public const string ListViews = "Application.ListViews";

        public static IModelList<IModelListView> Get_ListViews(IModelApplicationListViews modelClassFullTextSearch) {
            return new CalculatedModelNodeList<IModelListView>(modelClassFullTextSearch.Application.Views.OfType<IModelListView>());
        }
    }
    [ModelAbstractClass]
    public interface IModelColumnDetailViews : IModelColumn {
        [Browsable(false)]
        IModelList<IModelDetailView> DetailViews { get; }
    }
    [DomainLogic(typeof(IModelColumnDetailViews))]
    public class ModelColumnDetailViewsDomainLogic {
        public const string DetailViews = "DetailViews";
        public static IModelList<IModelDetailView> Get_DetailViews(IModelColumnDetailViews detailViews) {
            var modelDetailViews = ((ModelNode)detailViews).Application.Views.OfType<IModelDetailView>()
                                         .Where(view => detailViews.ModelMember.MemberInfo.MemberTypeInfo == view.ModelClass.TypeInfo);
            return new CalculatedModelNodeList<IModelDetailView>(modelDetailViews);
        }
    }

    [ModelAbstractClass]
    public interface IModelMemberDataStoreForeignKeyCreated : IModelMember {
        [Browsable((false))]
        bool DataStoreForeignKeyCreated { get; set; }
    }
    public interface IModelMemberCellFilter : IModelNode {
        [Category(AttributeCategoryNameProvider.Search)]
        bool CellFilter { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberCellFilter), "ModelMember")]
    public interface IModelColumnCellFilter : IModelMemberCellFilter {
    }

    public interface IColumnCellFilterUser {
         
    }
}
