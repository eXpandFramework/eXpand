using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.AdditionalViewControls;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Model {
    public class AdditionalViewControlsRulesNodeUpdater :LogicRulesNodeUpdater<IAdditionalViewControlsRule, IModelAdditionalViewControlsRule> {
        protected override void SetAttribute(IModelAdditionalViewControlsRule rule,IAdditionalViewControlsRule attribute) {
            rule.Attribute = attribute;
        }
    }
}