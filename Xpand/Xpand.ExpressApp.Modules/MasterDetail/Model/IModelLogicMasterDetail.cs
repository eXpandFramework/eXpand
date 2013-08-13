using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.MasterDetail.Model {

    [ModelLogicValidRule(typeof(IMasterDetailRule))]
    public interface IModelLogicMasterDetail : IModelLogic {

    }
}