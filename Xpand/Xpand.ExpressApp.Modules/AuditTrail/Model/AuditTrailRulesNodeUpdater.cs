using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.AuditTrail;

namespace Xpand.ExpressApp.AuditTrail.Model {
    public class AuditTrailRulesNodeUpdater : LogicRulesNodeUpdater<IAuditTrailRule, IModelAuditTrailRule> {
        protected override void SetAttribute(IModelAuditTrailRule rule, IAuditTrailRule attribute) {
            rule.Attribute = attribute;
        }

    }
}
