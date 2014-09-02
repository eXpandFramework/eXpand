using DevExpress.Persistent.AuditTrail;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.AuditTrail.Logic {
    public class AuditTrailRule:LogicRule,IAuditTrailRule {
        public AuditTrailRule(IContextAuditTrailRule auditTrailRule)
            : base(auditTrailRule) {
            IncludeRelatedTypes=auditTrailRule.IncludeRelatedTypes;
            AuditingMode=auditTrailRule.AuditingMode;
            AuditMemberStrategy = auditTrailRule.AuditMemberStrategy;
            MemberContext = auditTrailRule.AuditTrailMembersContext;
            AuditTrailStrategy=auditTrailRule.AuditTrailStrategy;
            AuditPending=auditTrailRule.AuditPending;
        }

        public string MemberContext { get; set; }

        public bool? AuditPending { get; set; }

        public bool IncludeRelatedTypes { get; set; }

        public ObjectAuditingMode? AuditingMode { get; set; }

        public AuditMemberStrategy AuditMemberStrategy { get; set; }

        public AuditTrailStrategy AuditTrailStrategy { get; set; }
    }

    public enum AuditMemberStrategy {
        None,
        OwnMembers,
        AllMembers
    }
}