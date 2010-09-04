using System;
using System.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic.Conditional.Security;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Security {
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
        public bool NotUseSameType { get; set; }
        #endregion
        public override IPermission Copy() {
            return new AdditionalViewControlsPermission();
        }
    }
}