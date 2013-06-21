using Xpand.ExpressApp.Logic.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ConditionalObjectView.Model {
    [ModelLogicRule(typeof(IModelConditionalObjectViewRule))]
    public interface IModelLogicConditionalObjectView : IModelLogic {
    }
}
