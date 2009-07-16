using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.Persistent.BaseImpl.Validation.RuleRequiredForAtLeast1Property;

namespace eXpand.ExpressApp.ModelArtifactState.Security.Permissions
{
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ControllerType")]
    [NonPersistent]
    public class ControllersStatePermission : ModelArtifactStatePermission, IControllerState
    {
        public override IPermission Copy()
        {
            return new ControllersStatePermission();
        }

        public override string ToString()
        {
            return string.Format("{2}: {0} {1}", ControllerType, Name, GetType().Name);
        }

//        public bool ApplyToDerivedController { get; set; }

        /// <summary>
        /// Type of controller to activate or not
        /// </summary>
        public string ControllerType { get; set; }
        
       
    }
}