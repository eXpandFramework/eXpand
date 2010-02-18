using DevExpress.Xpo;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState {
    public abstract class ArtifactStateRulePermission : RulePermission, IArtifactRule
    {
        #region IArtifactRule Members
        [DisplayName("Module (regex)")]
        public string Module { get; set; }
        #endregion
//        public static explicit operator ArtifactStateRule(ArtifactStateRulePermission artifactStateRulePermission) {
//            return artifactStateRulePermission is ControllerStateRulePermission
//                       ? (ArtifactStateRule) new ControllerStateRule(artifactStateRulePermission)
//                       : new ActionStateRule(artifactStateRulePermission);
//        }
    }
}