using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Security.Improved;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Security.Improved {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ActionId")]
    [System.ComponentModel.DisplayName("ActionState")]
    public class ActionStateOperationPermissionData : ArtifactStateOperationPermissionData, IContextActionStateRule {
        public ActionStateOperationPermissionData(Session session)
            : base(session) {
        }
        #region IActionStateRule Members
        public string ActionId { get; set; }

        public Logic.ActionState ActionState { get; set; }
        #endregion
        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new ActionStateRulePermission(this) };
        }
    }
}