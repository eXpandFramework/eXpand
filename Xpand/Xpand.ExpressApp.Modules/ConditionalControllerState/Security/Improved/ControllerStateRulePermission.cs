using System;
using System.Collections.Generic;
using Xpand.ExpressApp.ArtifactState.Logic;
using Xpand.ExpressApp.ArtifactState.Security.Improved;
using Xpand.ExpressApp.ConditionalControllerState.Logic;

namespace Xpand.ExpressApp.ConditionalControllerState.Security.Improved {
    public class ControllerStateRulePermission : ArtifactStateRulePermission, IControllerStateRule {
        public const string OperationName = "ActionState";
        public ControllerStateRulePermission(IControllerStateRule logicRule)
            : base(OperationName, logicRule) {
            ControllerState = logicRule.ControllerState;
            ControllerType = logicRule.ControllerType;
        }

        protected ControllerStateRulePermission(string operation, IArtifactStateRule logicRule)
            : base(operation, logicRule) {
        }
        #region IControllerStateRule Members
        public Type ControllerType { get; set; }

        public ControllerState ControllerState { get; set; }

        #endregion
        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
    }
}