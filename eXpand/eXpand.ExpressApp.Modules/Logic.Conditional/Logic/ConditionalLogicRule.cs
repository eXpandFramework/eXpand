namespace eXpand.ExpressApp.Logic.Conditional.Logic {
    public class ConditionalLogicRule : LogicRule, IConditionalLogicRule {
        

        public ConditionalLogicRule(IConditionalLogicRule controllerStateRule)
            : base(controllerStateRule) {
                NormalCriteria = controllerStateRule.NormalCriteria;
                EmptyCriteria = controllerStateRule.EmptyCriteria;
        }
        #region IConditionalLogicRule Members
        public string NormalCriteria { get; set; }

        public string EmptyCriteria { get; set; }
        #endregion
    }
}