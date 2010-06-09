namespace eXpand.ExpressApp.Logic.Conditional.Logic {
    public class ConditionalLogicRule : LogicRule, IConditionalLogicRule {
        readonly IConditionalLogicRule _controllerStateRule;

        public ConditionalLogicRule(IConditionalLogicRule controllerStateRule)
            : base(controllerStateRule) {
            _controllerStateRule = controllerStateRule;
        }
        #region IConditionalLogicRule Members
        public string NormalCriteria {
            get { return _controllerStateRule.NormalCriteria; }
            set { _controllerStateRule.NormalCriteria = value; }
        }

        public string EmptyCriteria {
            get { return _controllerStateRule.EmptyCriteria; }
            set { _controllerStateRule.EmptyCriteria = value; }
        }
        #endregion
    }
}