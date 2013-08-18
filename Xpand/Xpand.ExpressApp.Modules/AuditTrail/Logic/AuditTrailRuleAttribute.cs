using DevExpress.Persistent.AuditTrail;
using Xpand.ExpressApp.Logic;

namespace Xpand.ExpressApp.AuditTrail.Logic {
    public sealed class AuditTrailRuleAttribute:LogicRuleAttribute,IContextAuditTrailRule {
        public bool IncludedRelatedTypes { get; set; }

        public AuditTrailRuleAttribute(string id) : base(id) {
        }

        public bool IncludeRelatedTypes { get; set; }

        public ObjectAuditingMode? AuditingMode { get; set; }

        public bool AuditAllMembers { get; set; }

        public string AuditTrailMembersContext { get; set; }
    }
}