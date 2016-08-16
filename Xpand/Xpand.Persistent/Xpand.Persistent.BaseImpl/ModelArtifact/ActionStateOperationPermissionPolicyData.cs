using System;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.ModelArtifact;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.Persistent.BaseImpl.ModelArtifact {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ActionId")]
    [System.ComponentModel.DisplayName("ActionState")]
    public class ActionStateOperationPermissionPolicyData : ArtifactStateOperationPermissionPolicyData, IContextActionStateRule {
        public ActionStateOperationPermissionPolicyData(Session session)
            : base(session) {
        }
        #region IActionStateRule Members
        public string ActionId { get; set; }

        public ActionState ActionState { get; set; }
        #endregion

        protected override Type GetPermissionType(){
            return typeof(IContextActionStateRule);
        }

        public string ActionContext { get; set; }
    }
}