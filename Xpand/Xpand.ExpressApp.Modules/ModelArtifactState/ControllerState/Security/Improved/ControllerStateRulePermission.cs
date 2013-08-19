using System;
using System.Collections.Generic;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Security.Improved;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Security.Improved {
    public class ControllerStateRulePermission : ArtifactStateRulePermission, IContextControllerStateRule {
        public const string OperationName = "ControllerState";
        public ControllerStateRulePermission(ControllerStateOperationPermissionData logicRule)
            : base(OperationName, logicRule) {
            ControllerState = logicRule.ControllerState;
            ControllerType = logicRule.ControllerType;
        }

        protected ControllerStateRulePermission(string operation, ControllerStateOperationPermissionData logicRule)
            : base(operation, logicRule) {
        }
        #region IControllerStateRule Members
        public Type ControllerType { get; set; }

        public Logic.ControllerState ControllerState { get; set; }

        #endregion
        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
    }
}