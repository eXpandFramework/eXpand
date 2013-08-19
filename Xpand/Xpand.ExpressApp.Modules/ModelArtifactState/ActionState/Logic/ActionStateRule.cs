using System.ComponentModel;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public class ActionStateRule : ArtifactStateRule, IActionStateRule {
        public ActionStateRule(IContextActionStateRule actionStateRule)
            : base(actionStateRule) {
            ActionId = actionStateRule.ActionId;
            ActionState = actionStateRule.ActionState;
        }

        [Category("Data")]
        public string ActionId { get; set; }

        [Category("Behavior")]
        public ActionState ActionState { get; set; }
    }
}
