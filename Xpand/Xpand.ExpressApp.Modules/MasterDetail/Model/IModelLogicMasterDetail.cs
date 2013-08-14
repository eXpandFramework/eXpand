using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.MasterDetail.Model {

    [ModelLogicValidRule(typeof(IMasterDetailRule))]
    [LogicInstaller(typeof(MasterDetailLogicInstaller))]
    public interface IModelLogicMasterDetail : IModelLogic {

    }
}