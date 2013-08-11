using System;
using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ArtifactState.Security;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.ExpressApp.ConditionalControllerState.Security {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ControllerType")]
    [NonPersistent]
    public class ControllerStateRulePermission : ArtifactStateRulePermission, IControllerStateRule {
        #region IControllerStateRule Members
        public Type ControllerType { get; set; }

        public ControllerState ControllerState { get; set; }

        #endregion
        public override IPermission Copy() {
            return new ControllerStateRulePermission();
        }

        public override string ToString() {
            return string.Format("{2}: {0} {1}", ControllerType, ID, GetType().Name);
        }
    }
}