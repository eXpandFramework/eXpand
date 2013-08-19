using System.Collections.Generic;
using DevExpress.Persistent.AuditTrail;
using Xpand.ExpressApp.AuditTrail.Logic;
using Xpand.ExpressApp.Logic.Security.Improved;

namespace Xpand.ExpressApp.AuditTrail.Security {
    public class AuditTrailRulePermission:LogicRulePermission,IContextAuditTrailRule {
        public const string OperationName = "AuditTrail";

        public AuditTrailRulePermission(AuditTrailOperationPermissionData contextLogicRule)
            : base(OperationName, contextLogicRule) {
            IncludeRelatedTypes=contextLogicRule.IncludeRelatedTypes;
            AuditingMode=contextLogicRule.AuditingMode;
        }

        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }

        public bool IncludeRelatedTypes { get; set; }

        public ObjectAuditingMode? AuditingMode { get; set; }

        public bool AuditAllMembers { get; set; }

        public string AuditTrailMembersContext { get; set; }
    }
}