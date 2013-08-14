using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Security.Improved;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Security.Improved {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ControllerType")]
    [System.ComponentModel.DisplayName("ControllerState")]
    public class ControllerStateOperationPermissionData : ArtifactStateOperationPermissionData, IContextControllerStateRule {
        public ControllerStateOperationPermissionData(Session session)
            : base(session) {
        }
        #region IControllerStateRule Members
        public Type ControllerType { get; set; }

        public Logic.ControllerState ControllerState { get; set; }

        #endregion
        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new ControllerStateRulePermission(this) };
        }
    }
}
