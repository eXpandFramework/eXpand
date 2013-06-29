using System.ComponentModel;
using Xpand.ExpressApp.ArtifactState.Logic;

namespace Xpand.ExpressApp.ConditionalActionState.Logic {
    public class ActionStateRule : ArtifactStateRule, IActionStateRule {
        public ActionStateRule(IActionStateRule actionStateRule)
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
