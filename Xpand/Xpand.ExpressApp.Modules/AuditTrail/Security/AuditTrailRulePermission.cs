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
            AuditMemberStrategy = contextLogicRule.AuditMemberStrategy;
            AuditTrailMembersContext = contextLogicRule.AuditTrailMembersContext;

        }

        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }

        public bool IncludeRelatedTypes { get; set; }

        public ObjectAuditingMode? AuditingMode { get; set; }

        public AuditMemberStrategy AuditMemberStrategy { get; set; }

        public string AuditTrailMembersContext { get; set; }
    }
}