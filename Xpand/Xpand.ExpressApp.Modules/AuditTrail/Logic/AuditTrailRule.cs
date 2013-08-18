using DevExpress.Persistent.AuditTrail;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.AuditTrail.Logic {
    public class AuditTrailRule:LogicRule,IAuditTrailRule {
        public AuditTrailRule(IAuditTrailRule auditTrailRule)
            : base(auditTrailRule) {
            IncludeRelatedTypes=auditTrailRule.IncludeRelatedTypes;
            AuditingMode=auditTrailRule.AuditingMode;
            AuditAllMembers = auditTrailRule.AuditAllMembers;
        }

        public bool IncludeRelatedTypes { get; set; }

        public ObjectAuditingMode? AuditingMode { get; set; }

        public bool AuditAllMembers { get; set; }
    }
}