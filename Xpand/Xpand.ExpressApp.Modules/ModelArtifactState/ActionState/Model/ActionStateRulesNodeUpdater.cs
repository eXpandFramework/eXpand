using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.ModelArtifact;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Model {
    public class ActionStateRulesNodeUpdater :LogicRulesNodeUpdater<IActionStateRule, IModelActionStateRule> {
        protected override void SetAttribute(IModelActionStateRule rule,IActionStateRule attribute) {
            rule.Attribute = attribute;
        }

    }
}