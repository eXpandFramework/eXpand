using eXpand.ExpressApp.Logic.Conditional.Logic;
using eXpand.ExpressApp.Logic.Security;

namespace eXpand.ExpressApp.Logic.Conditional.Security {
    public abstract class ConditionalLogicRulePermission:LogicRulePermission,IConditionalLogicRule {
        public string NormalCriteria { get; set; }
        public string EmptyCriteria { get; set; }
    }
}