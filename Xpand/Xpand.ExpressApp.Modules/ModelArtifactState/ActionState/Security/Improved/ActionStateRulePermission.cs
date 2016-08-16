using System.Collections.Generic;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Security.Improved;
using Xpand.Persistent.Base.ModelArtifact;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Security.Improved {
    public class ActionStateRulePermission : ArtifactStateRulePermission, IContextActionStateRule {
        public const string OperationName = "ActionState";
        public ActionStateRulePermission(IContextActionStateRule logicRule)
            : base(OperationName, logicRule) {
            ActionId = logicRule.ActionId;
            ActionState = logicRule.ActionState;
            ActionContext = logicRule.ActionContext;
        }

        protected ActionStateRulePermission(string operation, IContextActionStateRule logicRule)
            : base(operation, logicRule) {
        }

        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
        #region IActionStateRule Members
        public string ActionId { get; set; }

        public Persistent.Base.ModelArtifact.ActionState ActionState { get; set; }

        #endregion

        public string ActionContext { get; set; }
    }
}
