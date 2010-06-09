using System.ComponentModel;
using eXpand.ExpressApp.ArtifactState.Logic;

namespace eXpand.ExpressApp.ConditionalActionState.Logic{
    public class ActionStateRule : ArtifactStateRule, IActionStateRule
    {
        readonly IActionStateRule _actionStateRule;

        public ActionStateRule(IActionStateRule controllerStateRule)
            : base(controllerStateRule)
        {
            _actionStateRule = controllerStateRule;
        }
        
        [Category("Data")]
        public string ActionId {
            get { return _actionStateRule.ActionId; }
            set { _actionStateRule.ActionId = value; }
        }
        [Category("Behavior")]
        public ActionState ActionState {
            get { return _actionStateRule.ActionState; }
            set { _actionStateRule.ActionState = value; }
        }
    }
}
