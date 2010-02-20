using eXpand.ExpressApp.ModelArtifactState.ArtifactState;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public class ActionStateRule : ArtifactStateRule, IActionStateRule {
        public ActionStateRule(IActionStateRule actionStateRule) : base(actionStateRule) {
        }

        public new IActionStateRule ArtifactRule {
            get { return base.ArtifactRule as IActionStateRule; }
        }
        #region IActionStateRule Members
        public string ActionId {
            get { return ArtifactRule.ActionId; }
            set { ArtifactRule.ActionId = value; }
        }

        public ActionState ActionState {
            get { return ArtifactRule.ActionState; }
            set { ArtifactRule.ActionState=value; }
        }
        #endregion
    }
}