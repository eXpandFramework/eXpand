using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.Logic.Conditional {
    public abstract class ConditionalLogicRulePermission:LogicRulePermission,IConditionalLogicRule {
        public string NormalCriteria { get; set; }
        public string EmptyCriteria { get; set; }
    }
}