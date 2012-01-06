using DevExpress.Xpo;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.Security.Improved;

namespace Xpand.ExpressApp.Logic.Conditional.Security.Improved {
    [NonPersistent]
    public abstract class ConditionalLogicOperationPermissionData : LogicRuleOperationPermissionData, IConditionalLogicRule {

        protected ConditionalLogicOperationPermissionData(Session session)
            : base(session) {
        }
        public string NormalCriteria { get; set; }
        public string EmptyCriteria { get; set; }
    }
}