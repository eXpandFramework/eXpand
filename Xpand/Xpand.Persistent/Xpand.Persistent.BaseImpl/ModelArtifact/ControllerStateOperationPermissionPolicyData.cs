using System;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.ModelArtifact;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.Persistent.BaseImpl.ModelArtifact {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ControllerType")]
    [System.ComponentModel.DisplayName("ControllerState")]
    public class ControllerStateOperationPermissionPolicyData : ArtifactStateOperationPermissionPolicyData, IContextControllerStateRule {
        public ControllerStateOperationPermissionPolicyData(Session session)
            : base(session) {
        }
        #region IControllerStateRule Members
        public Type ControllerType { get; set; }

        public ControllerState ControllerState { get; set; }

        #endregion

        protected override Type GetPermissionType(){
            return typeof(IContextControllerStateRule);
        }
    }
}
