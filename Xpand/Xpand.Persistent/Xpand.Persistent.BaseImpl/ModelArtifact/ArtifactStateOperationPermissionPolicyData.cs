using DevExpress.Xpo;
using Xpand.Persistent.Base.ModelArtifact;
using Xpand.Persistent.BaseImpl.Security.PermissionPolicyData;

namespace Xpand.Persistent.BaseImpl.ModelArtifact {
    [NonPersistent]
    public abstract class ArtifactStateOperationPermissionPolicyData : LogicRuleOperationPermissionPolicyData, IArtifactStateRule {
        protected ArtifactStateOperationPermissionPolicyData(Session session)
            : base(session) {
        }
        #region IArtifactRule Members
        [DisplayName("Module (regex)")]
        public string Module { get; set; }
        #endregion
    }
}