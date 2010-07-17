using System;
using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic.Conditional.Security;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Security {
    [NonPersistent]
    public class AdditionalViewControlsPermission : ConditionalLogicRulePermission, IAdditionalViewControlsRule {
        [RuleRequiredField]
        public Type ControlType { get; set; }
        [RuleRequiredField]
        public Type DecoratorType { get; set; }
       
        #region IAdditionalViewControlsRule Members
        
        public string Message { get; set; }
        public string MessageProperty { get; set; }

        public Position Position { get; set; }
        
        #endregion
        public override IPermission Copy() {
            return new AdditionalViewControlsPermission();
        }
    }
}