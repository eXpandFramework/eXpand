using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.Security.Improved;

namespace Xpand.ExpressApp.Logic.Conditional.Security.Improved {
    public abstract class ConditionalLogicRulePermissionRequest : LogicRulePermissionRequest, IConditionalLogicRule {
        protected ConditionalLogicRulePermissionRequest(string operation, IConditionalLogicRule logicRule)
            : base(operation, logicRule) {
            NormalCriteria = logicRule.NormalCriteria;
            EmptyCriteria = logicRule.EmptyCriteria;
        }
        public string NormalCriteria { get; set; }
        public string EmptyCriteria { get; set; }
    }
}