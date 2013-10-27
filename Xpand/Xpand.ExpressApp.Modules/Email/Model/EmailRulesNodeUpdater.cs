using Xpand.ExpressApp.Email.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.Email.Model {
    public class EmailRulesNodeUpdater : LogicRulesNodeUpdater<IEmailRule, IModelEmailRule> {
        protected override void SetAttribute(IModelEmailRule rule, IEmailRule attribute) {
            rule.Attribute = attribute;
        }
    }
}