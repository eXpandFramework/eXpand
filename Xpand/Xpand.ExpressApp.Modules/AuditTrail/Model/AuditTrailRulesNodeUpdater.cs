using Xpand.ExpressApp.AuditTrail.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.AuditTrail.Model {
    public class AuditTrailRulesNodeUpdater : LogicRulesNodeUpdater<IAuditTrailRule, IModelAuditTrailRule> {
        protected override void SetAttribute(IModelAuditTrailRule rule, IAuditTrailRule attribute) {
            rule.Attribute = attribute;
        }

    }
}
