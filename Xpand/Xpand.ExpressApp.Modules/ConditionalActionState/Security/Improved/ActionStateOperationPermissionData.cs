using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ArtifactState.Security.Improved;
using Xpand.ExpressApp.ConditionalActionState.Logic;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.ExpressApp.ConditionalActionState.Security.Improved {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ActionId")]
    public class ActionStateOperationPermissionData : ArtifactStateOperationPermissionData, IActionStateRule {
        public ActionStateOperationPermissionData(Session session)
            : base(session) {
        }
        #region IActionStateRule Members
        public string ActionId { get; set; }

        public ActionState ActionState { get; set; }
        #endregion
        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new ActionStateRulePermission(this) };
        }
    }
}