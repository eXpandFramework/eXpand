using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Security;
using Xpand.Persistent.Base.ModelArtifact;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Security {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ActionId")]
    [NonPersistent]
    public class ActionStateRulePermission : ArtifactStateRulePermission, IActionStateRule {
        #region IActionStateRule Members
        public virtual string ActionId { get; set; }

        public Persistent.Base.ModelArtifact.ActionState ActionState { get; set; }
        #endregion
        public override IPermission Copy() {
            return new ActionStateRulePermission();
        }

        public override string ToString() {
            return string.Format("{2}: {0} {1}", ActionId, ID, GetType().Name);
        }
    }
}