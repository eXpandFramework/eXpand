using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Model {
    public class ActionStateRulesNodeUpdater :LogicRulesNodeUpdater<IActionStateRule, IModelActionStateRule> {
        protected override void SetAttribute(IModelActionStateRule rule,IActionStateRule attribute) {
            rule.Attribute = attribute;
        }

    }
}