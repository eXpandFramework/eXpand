using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.Email;

namespace Xpand.ExpressApp.Email.Model {
    public class EmailRulesNodeUpdater : LogicRulesNodeUpdater<IEmailRule, IModelEmailRule> {
        protected override void SetAttribute(IModelEmailRule rule, IEmailRule attribute) {
            rule.Attribute = attribute;
        }
    }
}