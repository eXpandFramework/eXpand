using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.Persistent.BaseImpl.Validation.RuleRequiredForAtLeast1Property;

namespace eXpand.ExpressApp.ModelArtifactState.Security.Permissions
{
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ControllerType")]
    [NonPersistent]
    public class ControllerStateRulePermission : ArtifactStateRulePermission, IControllerStateRule
    {
        public override IPermission Copy()
        {
            return new ControllerStateRulePermission();
        }

        public override string ToString()
        {
            return string.Format("{2}: {0} {1}", ControllerType, ID, GetType().Name);
        }



        /// <summary>
        /// Type of controller to activate or not
        /// </summary>
        public virtual string ControllerType { get; set; }
        
       
    }
}