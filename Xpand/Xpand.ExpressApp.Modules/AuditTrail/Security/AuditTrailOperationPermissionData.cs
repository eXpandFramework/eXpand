using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Xpo;
using Xpand.ExpressApp.AuditTrail.Logic;
using Xpand.ExpressApp.Logic.Security.Improved;

namespace Xpand.ExpressApp.AuditTrail.Security {
    public class AuditTrailOperationPermissionData : LogicRuleOperationPermissionData,IContextAuditTrailRule {
        public AuditTrailOperationPermissionData(Session session) : base(session) {
        }

        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new AuditTrailRulePermission(this) };
        }

        public bool IncludeRelatedTypes { get; set; }

        public ObjectAuditingMode? AuditingMode { get; set; }

        public bool AuditAllMembers { get; set; }

        public string AuditTrailMembersContext { get; set; }
    }
}