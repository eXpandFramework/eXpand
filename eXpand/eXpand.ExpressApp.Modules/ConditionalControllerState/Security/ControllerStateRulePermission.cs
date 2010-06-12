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
        Type _controllerType;
        public string ControllerType {
            get { return XafTypesInfo.Instance.FindTypeInfo(((IControllerStateRule) this).ControllerType).FullName; }
            set { _controllerType = XafTypesInfo.Instance.FindTypeInfo(value).Type; }
        }

        Type IControllerStateRule.ControllerType {
            get { return _controllerType; }
            set { _controllerType=value; }
        }

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