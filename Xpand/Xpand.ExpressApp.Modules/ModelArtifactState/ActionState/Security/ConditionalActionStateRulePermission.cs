using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ConditionalActionState.Logic;
using eXpand.ExpressApp.Logic.Conditional.Security;
using eXpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace eXpand.ExpressApp.ConditionalActionState.Security {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ActionId")]
    [NonPersistent]
    public class ConditionalActionStateRulePermission : ConditionalLogicRulePermission, IConditionalActionStateRule {
        #region IActionStateRule Members
        public virtual string ActionId { get; set; }

        public ActionState ActionState { get; set; }
        #endregion
        public override IPermission Copy() {
            return new ConditionalActionStateRulePermission();
        }

        public override string ToString() {
            return string.Format("{2}: {0} {1}", ActionId, ID, GetType().Name);
        }
        [DisplayName("Module (regex)")]
        public string Module { get; set; }
    }
}