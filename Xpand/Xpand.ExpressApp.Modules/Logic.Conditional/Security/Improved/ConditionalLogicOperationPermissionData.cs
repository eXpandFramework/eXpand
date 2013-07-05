using DevExpress.Xpo;
using Xpand.ExpressApp.Logic.Security.Improved;
using Xpand.Persistent.Base.Logic;

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