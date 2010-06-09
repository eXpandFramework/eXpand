using System;
using System.Security;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic.Conditional.Security;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Security {
    [NonPersistent]
    public class AdditionalViewControlsPermission : ConditionalLogicRulePermission, IAdditionalViewControlsRule {

        public Type ControlType { get; set; }
        public Type DecoratorType { get; set; }
       
        #region IAdditionalViewControlsRule Members
        
        public string Message { get; set; }
        public string MessageProperty { get; set; }

        public Position Position { get; set; }
        public bool UseSameIfFound { get; set; }
        #endregion
        public override IPermission Copy() {
            return new AdditionalViewControlsPermission();
        }
    }
}