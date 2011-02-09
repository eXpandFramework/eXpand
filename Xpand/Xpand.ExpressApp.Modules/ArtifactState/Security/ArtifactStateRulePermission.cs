using DevExpress.Xpo;
using Xpand.ExpressApp.ArtifactState.Logic;
using Xpand.ExpressApp.Logic.Conditional.Security;

namespace Xpand.ExpressApp.ArtifactState.Security {
    public abstract class ArtifactStateRulePermission : ConditionalLogicRulePermission, IArtifactStateRule {
        #region IArtifactRule Members
        [DisplayName("Module (regex)")]
        public string Module { get; set; }
        #endregion
    }
}