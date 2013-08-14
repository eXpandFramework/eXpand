using Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model {

    [ModelLogicValidRuleAttribute(typeof(IObjectViewRule))]
    [LogicInstaller(typeof(ObjectViewsLogicInstaller))]
    public interface IModelLogicConditionalObjectView : IModelLogic {
    }
}
