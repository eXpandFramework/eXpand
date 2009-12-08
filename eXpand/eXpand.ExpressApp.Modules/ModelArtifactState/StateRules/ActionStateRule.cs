using eXpand.ExpressApp.ModelArtifactState.Interfaces;

namespace eXpand.ExpressApp.ModelArtifactState.StateRules
{
    public class ActionStateRule:ArtifactStateRule,IActionStateRule
    {        
        public ActionStateRule(IArtifactStateRule actionStateRule) : base(actionStateRule)
        {
            

        }
        
        public new IActionStateRule ArtifactRule
        {
            get { return base.ArtifactRule as IActionStateRule; }
        }

        public string ActionId
        {
            get { return ArtifactRule.ActionId; }
            set { ArtifactRule.ActionId = value; }
        }

    }
}