using System;
using System.Collections.Generic;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Security.Improved;
using Xpand.Persistent.Base.ModelArtifact;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Security.Improved {
    public class ControllerStateRulePermission : ArtifactStateRulePermission, IContextControllerStateRule {
        public const string OperationName = "ControllerState";
        public ControllerStateRulePermission(IContextControllerStateRule logicRule)
            : base(OperationName, logicRule) {
            ControllerState = logicRule.ControllerState;
            ControllerType = logicRule.ControllerType;
        }

        protected ControllerStateRulePermission(string operation, IContextControllerStateRule logicRule)
            : base(operation, logicRule) {
        }
        #region IControllerStateRule Members
        public Type ControllerType { get; set; }

        public Persistent.Base.ModelArtifact.ControllerState ControllerState { get; set; }

        #endregion
        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
    }
}