using System;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ArtifactState.Security;
using eXpand.ExpressApp.ConditionalControllerState.Logic;
using eXpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace eXpand.ExpressApp.ConditionalControllerState.Security {
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Module,ControllerType")]
    [NonPersistent]
    public class ControllerStateRulePermission : ArtifactStateRulePermission, IControllerStateRule
    {
        #region IControllerStateRule Members
        public Type ControllerType { get; set; }

        public ControllerState State { get; set; }
        #endregion
        public override IPermission Copy()
        {
            return new ControllerStateRulePermission();
        }

        public override string ToString()
        {
            return string.Format("{2}: {0} {1}", ControllerType, ID, GetType().Name);
        }
    }
}