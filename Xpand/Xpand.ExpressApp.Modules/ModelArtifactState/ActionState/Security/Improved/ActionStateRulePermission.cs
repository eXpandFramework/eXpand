using System.Collections.Generic;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Security.Improved;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Security.Improved {
    public class ActionStateRulePermission : ArtifactStateRulePermission, IContextActionStateRule {
        public const string OperationName = "ActionState";
        public ActionStateRulePermission(ActionStateOperationPermissionData logicRule)
            : base(OperationName, logicRule) {
            ActionId = logicRule.ActionId;
            ActionState = logicRule.ActionState;
        }

        protected ActionStateRulePermission(string operation, ActionStateOperationPermissionData logicRule)
            : base(operation, logicRule) {
        }

        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
        #region IActionStateRule Members
        public string ActionId { get; set; }

        public Logic.ActionState ActionState { get; set; }
        #endregion
    }
}
