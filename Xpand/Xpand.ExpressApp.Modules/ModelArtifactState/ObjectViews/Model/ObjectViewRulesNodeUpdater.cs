using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model {
    public class ObjectViewRulesNodeUpdater :LogicRulesNodeUpdater<IObjectViewRule, IModelObjectViewRule> {
        protected override void SetAttribute(IModelObjectViewRule rule, IObjectViewRule attribute) {
            rule.Attribute = attribute;
        }

    }
}