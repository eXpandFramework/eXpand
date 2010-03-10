using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;
using eXpand.Persistent.Base.General;
using eXpand.Persistent.BaseImpl.Validation.RuleRequiredForAtLeast1Property;

namespace eXpand.ExpressApp.ModelArtifactState.ControllerState.Logic {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ControllerType")]
    [NonPersistent]
    public class ControllerStateRulePermission : ArtifactStateRulePermission, IControllerStateRule {
        #region IControllerStateRule Members
        /// <summary>
        /// Type of controller to activate or not
        /// </summary>
        public string ControllerType { get; set; }

        public State State { get; set; }
        #endregion
        public override IPermission Copy() {
            return new ControllerStateRulePermission();
        }

        public override string ToString() {
            return string.Format("{2}: {0} {1}", ControllerType, ID, GetType().Name);
        }
    }
}