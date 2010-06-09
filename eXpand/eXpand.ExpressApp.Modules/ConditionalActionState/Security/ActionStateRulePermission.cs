using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ArtifactState.Security;
using eXpand.ExpressApp.ConditionalActionState.Logic;
using eXpand.Persistent.BaseImpl.Validation.RuleRequiredForAtLeast1Property;

namespace eXpand.ExpressApp.ConditionalActionState.Security {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ActionId")]
    [NonPersistent]
    public class ActionStateRulePermission : ArtifactStateRulePermission, IActionStateRule {
        #region IActionStateRule Members
        public virtual string ActionId { get; set; }

        public ActionState ActionState { get; set; }
        #endregion
        public override IPermission Copy() {
            return new ActionStateRulePermission();
        }

        public override string ToString() {
            return string.Format("{2}: {0} {1}", ActionId, ID, GetType().Name);
        }
    }
}