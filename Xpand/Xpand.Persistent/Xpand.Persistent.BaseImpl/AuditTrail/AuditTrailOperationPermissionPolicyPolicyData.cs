using System;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Xpo;
using Xpand.Persistent.Base.AuditTrail;
using Xpand.Persistent.BaseImpl.Security.PermissionPolicyData;
using ObjectAuditingMode = Xpand.Persistent.Base.AuditTrail.ObjectAuditingMode;


namespace Xpand.Persistent.BaseImpl.AuditTrail {
    [System.ComponentModel.DisplayName("AuditTrail")]
    public class AuditTrailOperationPermissionPolicyPolicyData : LogicRuleOperationPermissionPolicyData,IContextAuditTrailRule {
        public AuditTrailOperationPermissionPolicyPolicyData(Session session) : base(session) {
        }

        public bool? AuditPending { get; set; }

        public bool IncludeRelatedTypes { get; set; }

        public ObjectAuditingMode? AuditingMode { get; set; }

        public AuditMemberStrategy AuditMemberStrategy { get; set; }

        public AuditTrailStrategy AuditTrailStrategy { get; set; }

        public string AuditTrailMembersContext { get; set; }
        protected override Type GetPermissionType(){
            return typeof(IContextAuditTrailRule);
        }
    }
}