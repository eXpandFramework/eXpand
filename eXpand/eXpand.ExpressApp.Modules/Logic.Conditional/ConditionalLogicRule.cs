namespace eXpand.ExpressApp.Logic.Conditional {
    public class ConditionalLogicRule : LogicRule, IConditionalLogicRule {
        readonly IConditionalLogicRule _conditionalLogicRule;

        public ConditionalLogicRule(IConditionalLogicRule conditionalLogicRule)
            : base(conditionalLogicRule) {
            _conditionalLogicRule = conditionalLogicRule;
        }
        #region IConditionalLogicRule Members
        public string NormalCriteria {
            get { return _conditionalLogicRule.NormalCriteria; }
            set { _conditionalLogicRule.NormalCriteria = value; }
        }

        public string EmptyCriteria {
            get { return _conditionalLogicRule.EmptyCriteria; }
            set { _conditionalLogicRule.EmptyCriteria = value; }
        }
        #endregion
    }
}