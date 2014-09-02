using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Xpo;
using Xpand.ExpressApp.AuditTrail.Logic;
using Xpand.ExpressApp.Logic.Security.Improved;
using ObjectAuditingMode = Xpand.ExpressApp.AuditTrail.Logic.ObjectAuditingMode;

namespace Xpand.ExpressApp.AuditTrail.Security {
    [System.ComponentModel.DisplayName("AuditTrail")]
    public class AuditTrailOperationPermissionData : LogicRuleOperationPermissionData,IContextAuditTrailRule {
        public AuditTrailOperationPermissionData(Session session) : base(session) {
        }

        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new AuditTrailRulePermission(this) };
        }

        public bool? AuditPending { get; set; }

        public bool IncludeRelatedTypes { get; set; }

        public ObjectAuditingMode? AuditingMode { get; set; }

        public AuditMemberStrategy AuditMemberStrategy { get; set; }

        public AuditTrailStrategy AuditTrailStrategy { get; set; }

        public string AuditTrailMembersContext { get; set; }
    }
}