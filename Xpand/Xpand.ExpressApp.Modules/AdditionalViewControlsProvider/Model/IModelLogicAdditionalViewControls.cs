using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Model {
    [ModelLogicValidRule(typeof(IAdditionalViewControlsRule))]
    public interface IModelLogicAdditionalViewControls:IModelLogic {
        
    }
}