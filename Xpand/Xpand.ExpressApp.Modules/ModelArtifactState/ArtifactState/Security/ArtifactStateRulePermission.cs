using DevExpress.Xpo;
using Xpand.ExpressApp.Logic.Security;
using Xpand.Persistent.Base.ModelArtifact;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Security {
    public abstract class ArtifactStateRulePermission : LogicRulePermission, IArtifactStateRule {
        #region IArtifactRule Members
        [DisplayName("Module (regex)")]
        public string Module { get; set; }
        #endregion
    }
}