using Xpand.ExpressApp.Logic.Security;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic.Conditional.Security {
    public abstract class ConditionalLogicRulePermission:LogicRulePermission,IConditionalLogicRule {
        public string NormalCriteria { get; set; }
        public string EmptyCriteria { get; set; }
    }
}