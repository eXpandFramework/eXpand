using System;
using System.Security;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider
{
    [NonPersistent]
    public class AdditionalViewControlsPermission : RulePermission,IAdditionalViewControlsRule
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
