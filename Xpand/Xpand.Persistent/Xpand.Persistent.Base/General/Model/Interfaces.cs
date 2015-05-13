using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.Persistent.Base.General.Model {
    
    public interface IModelApplicationViews:IModelNode {
        [Browsable(false)]
        IModelList<IModelListView> ListViews { get; }
        [Browsable(false)]
        IModelList<IModelDetailView> DetailViews { get; }
        [Browsable(false)]
        IModelList<IModelDashboardView> DashboardViews { get; }
    }
    [DomainLogic(typeof(IModelApplicationViews))]
    public class ModelApplicationViewsDomainLogic {
        public const string ListViews = "Application.ListViews";
        public const string DetailViews = "Application.DetailViews";
        public const string DashboardViews = "Application.DashboardViews";

        public static IModelList<IModelListView> Get_ListViews(IModelApplicationViews modelClassFullTextSearch){
            return GetViews<IModelListView>(modelClassFullTextSearch);
        }

        private static IModelList<T> GetViews<T>(IModelApplicationViews modelClassFullTextSearch) where T : IModelView{
            return new CalculatedModelNodeList<T>(modelClassFullTextSearch.Application.Views.OfType<T>());
        }

        public static IModelList<IModelDetailView> Get_DetailViews(IModelApplicationViews modelClassFullTextSearch) {
            return GetViews<IModelDetailView>(modelClassFullTextSearch);
        }

        public static IModelList<IModelDashboardView> Get_DashboardViews(IModelApplicationViews modelClassFullTextSearch) {
            return GetViews<IModelDashboardView>(modelClassFullTextSearch);
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
