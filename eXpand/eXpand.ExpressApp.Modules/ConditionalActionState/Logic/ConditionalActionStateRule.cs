using System.ComponentModel;
using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.ConditionalActionState.Logic{
    public class ConditionalActionStateRule : ConditionalLogicRule, IConditionalActionStateRule
    {
        readonly IConditionalActionStateRule _conditionalActionStateRule;

        public ConditionalActionStateRule(IConditionalActionStateRule controllerStateRule)
            : base(controllerStateRule)
        {
            _conditionalActionStateRule = controllerStateRule;
        }
        public string Module
        {
            get { return _conditionalActionStateRule.Module; }
            set { _conditionalActionStateRule.Module = value; }
        }

        [Category("Data")]
        public string ActionId {
            get { return _conditionalActionStateRule.ActionId; }
            set { _conditionalActionStateRule.ActionId = value; }
        }
        [Category("Behavior")]
        public ActionState ActionState {
            get { return _conditionalActionStateRule.ActionState; }
            set { _conditionalActionStateRule.ActionState = value; }
        }
    }
}
