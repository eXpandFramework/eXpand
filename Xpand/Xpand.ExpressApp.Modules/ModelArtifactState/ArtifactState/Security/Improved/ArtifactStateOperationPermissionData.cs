using DevExpress.Xpo;
using Xpand.ExpressApp.Logic.Security.Improved;
using Xpand.Persistent.Base.ModelArtifact;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Security.Improved {
    [NonPersistent]
    public abstract class ArtifactStateOperationPermissionData : LogicRuleOperationPermissionData, IArtifactStateRule {
        protected ArtifactStateOperationPermissionData(Session session)
            : base(session) {
        }
        #region IArtifactRule Members
        [DisplayName("Module (regex)")]
        public string Module { get; set; }
        #endregion
    }
}