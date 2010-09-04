using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.Security;

namespace Xpand.ExpressApp.Logic.Conditional.Security {
    public abstract class ConditionalLogicRulePermission:LogicRulePermission,IConditionalLogicRule {
        public string NormalCriteria { get; set; }
        public string EmptyCriteria { get; set; }
    }
}