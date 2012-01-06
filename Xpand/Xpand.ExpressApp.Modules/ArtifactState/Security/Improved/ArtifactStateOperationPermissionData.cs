using DevExpress.Xpo;
using Xpand.ExpressApp.ArtifactState.Logic;
using Xpand.ExpressApp.Logic.Conditional.Security.Improved;

namespace Xpand.ExpressApp.ArtifactState.Security.Improved {
    [NonPersistent]
    public abstract class ArtifactStateOperationPermissionData : ConditionalLogicOperationPermissionData, IArtifactStateRule {
        protected ArtifactStateOperationPermissionData(Session session)
            : base(session) {
        }
        #region IArtifactRule Members
        [DisplayName("Module (regex)")]
        public string Module { get; set; }
        #endregion
    }
}