using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.Persistent.Base.General.Model {
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

}
