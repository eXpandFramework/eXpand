using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model {
    [ModelAbstractClass]
    public interface IModelColumnDetailViews : IModelColumn {
        [Browsable(false)]
        IModelList<IModelDetailView> DetailViews { get; }
    }
    [ModelAbstractClass]
    public interface IModelMemberDataStoreForeignKeyCreated : IModelMember {
        [Browsable((false))]
        bool DataStoreForeignKeyCreated { get; set; }
    }

}
