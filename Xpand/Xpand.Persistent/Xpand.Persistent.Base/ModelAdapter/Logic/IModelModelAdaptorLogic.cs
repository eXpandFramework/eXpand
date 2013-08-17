using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.Persistent.Base.ModelAdapter.Logic {
    [ModelLogicValidRule(typeof(IModelAdaptorRule))]
    public interface IModelModelAdaptorLogic : IModelLogic {
    }

    public interface IModelAdaptorLogicIntaller:ILogicInstaller {
         
    }
}
