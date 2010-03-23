using System;
using System.Security;
using DevExpress.Xpo;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic
{
    [NonPersistent]
    public class AdditionalViewControlsPermission : ConditionalLogicRulePermission,IAdditionalViewControlsRule
    {
        public override IPermission Copy() {
            return new AdditionalViewControlsPermission();
        }

        public string Message { get; set; }
        public string MessagePropertyName { get; set; }
        public Type DecoratorType { get; set; }
        public Type ControlType { get; set; }
        public AdditionalViewControlsProviderPosition AdditionalViewControlsProviderPosition { get; set; }
        public bool UseSameIfFound { get; set; }
    }
}


