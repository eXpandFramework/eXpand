using DevExpress.Xpo;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.ModelArtifactState.StateRules;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.Security.Permissions
{
    public abstract class ArtifactStateRulePermission:StatePermission,IArtifactStateRule
    {
        [DisplayName("Module (regex)")]
        public string Module { get; set; }
        public static explicit operator ArtifactStateRule(ArtifactStateRulePermission artifactStateRulePermission)  
        {

            return artifactStateRulePermission is ControllerStateRulePermission ? (ArtifactStateRule)new ControllerStateRule(artifactStateRulePermission) : new ActionStateRule(artifactStateRulePermission);
        }

    }
}