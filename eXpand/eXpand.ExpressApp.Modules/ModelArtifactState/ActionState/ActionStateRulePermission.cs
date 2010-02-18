using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState;
using eXpand.Persistent.BaseImpl.Validation.RuleRequiredForAtLeast1Property;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState
{
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ActionId")]
    [NonPersistent]
    public class ActionStateRulePermission : ArtifactStateRulePermission,IActionStateRule
    {
        public override IPermission Copy()
        {
            return new ActionStateRulePermission();
        }
        public override string ToString()
        {
            return string.Format("{2}: {0} {1}", ActionId, ID, GetType().Name);
        }
        public virtual string ActionId { get; set; }

        public ActionState ActionState { get; set; }
    }
}