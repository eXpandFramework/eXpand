using System.Collections.Generic;
using DevExpress.Persistent.AuditTrail;
using Xpand.ExpressApp.Logic.Security.Improved;
using Xpand.Persistent.Base.AuditTrail;
using ObjectAuditingMode = Xpand.Persistent.Base.AuditTrail.ObjectAuditingMode;

namespace Xpand.ExpressApp.AuditTrail.Security {
    public class AuditTrailRulePermission:LogicRulePermission,IContextAuditTrailRule {
        public const string OperationName = "AuditTrail";

        public AuditTrailRulePermission(IContextAuditTrailRule contextLogicRule)
            : base(OperationName, contextLogicRule) {
            IncludeRelatedTypes=contextLogicRule.IncludeRelatedTypes;
            AuditPending = contextLogicRule.AuditPending;
            AuditingMode=contextLogicRule.AuditingMode;
            AuditMemberStrategy = contextLogicRule.AuditMemberStrategy;
            AuditTrailMembersContext = contextLogicRule.AuditTrailMembersContext;
            AuditTrailStrategy = contextLogicRule.AuditTrailStrategy;
        }

        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }

        public bool? AuditPending { get; set; }

        public bool IncludeRelatedTypes { get; set; }

        public ObjectAuditingMode? AuditingMode { get; set; }

        public AuditMemberStrategy AuditMemberStrategy { get; set; }

        public AuditTrailStrategy AuditTrailStrategy { get; set; }

        public string AuditTrailMembersContext { get; set; }
    }
}