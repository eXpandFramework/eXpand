using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.BaseImpl.Validation.RuleRequiredForAtLeast1Property;

namespace eXpand.ExpressApp.ModelArtifactState.Security.Permissions
{
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ActionId")]
    [NonPersistent]
    
    public class ActionsStatePermission : ModelArtifactStatePermission
    {
        public override IPermission Copy()
        {
            return new ActionsStatePermission();
        }
        public override string ToString()
        {
            return string.Format("{2}: {0} {1}", ActionId, Name, GetType().Name);
        }
        public virtual string ActionId { get; set; }
        
    }
}