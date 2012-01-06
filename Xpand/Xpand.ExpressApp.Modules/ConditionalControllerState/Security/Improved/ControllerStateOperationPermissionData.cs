using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ArtifactState.Security.Improved;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.ExpressApp.ConditionalControllerState.Security.Improved {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ControllerType")]
    public class ControllerStateOperationPermissionData : ArtifactStateOperationPermissionData, IControllerStateRule {
        public ControllerStateOperationPermissionData(Session session)
            : base(session) {
        }
        #region IControllerStateRule Members
        public Type ControllerType { get; set; }

        public ControllerState ControllerState { get; set; }
        #endregion
        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new ControllerStateRulePermission(this) };
        }
    }
}
