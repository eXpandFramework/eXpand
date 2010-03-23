using DevExpress.Xpo;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic {
    public abstract class ArtifactStateRulePermission : ConditionalLogicRulePermission, IArtifactRule
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