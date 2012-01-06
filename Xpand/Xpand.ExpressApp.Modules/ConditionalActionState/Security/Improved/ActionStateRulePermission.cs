using System.Collections.Generic;
using Xpand.ExpressApp.ArtifactState.Logic;
using Xpand.ExpressApp.ArtifactState.Security.Improved;
using Xpand.ExpressApp.ConditionalActionState.Logic;

namespace Xpand.ExpressApp.ConditionalActionState.Security.Improved {
    public class ActionStateRulePermission : ArtifactStateRulePermission, IActionStateRule {
        public const string OperationName = "ActionState";
        public ActionStateRulePermission(IActionStateRule logicRule)
            : base(OperationName, logicRule) {
            ActionId = logicRule.ActionId;
            ActionState = logicRule.ActionState;
        }

        protected ActionStateRulePermission(string operation, IArtifactStateRule logicRule)
            : base(operation, logicRule) {
        }

        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
        #region IActionStateRule Members
        public string ActionId { get; set; }

        public ActionState ActionState { get; set; }
        #endregion
    }
}
