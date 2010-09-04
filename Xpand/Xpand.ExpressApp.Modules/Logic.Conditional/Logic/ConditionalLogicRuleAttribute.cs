namespace Xpand.ExpressApp.Logic.Conditional.Logic {
    public abstract class ConditionalLogicRuleAttribute:LogicRuleAttribute,IConditionalLogicRule {
        protected ConditionalLogicRuleAttribute(string id, string normalCriteria, string emptyCriteria) : base(id) {
            NormalCriteria = normalCriteria;
            EmptyCriteria = emptyCriteria;
        }

        public string NormalCriteria { get; set; }
        public string EmptyCriteria { get; set; }
    }
}