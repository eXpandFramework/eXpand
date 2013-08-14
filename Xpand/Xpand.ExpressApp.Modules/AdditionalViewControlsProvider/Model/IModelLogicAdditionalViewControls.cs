using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Model {
    [ModelLogicValidRule(typeof(IAdditionalViewControlsRule))]
    [LogicInstaller(typeof(AdditionalViewControlsLogicInstaller))]
    public interface IModelLogicAdditionalViewControls:IModelLogic {
        
    }
}