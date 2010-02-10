using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Decorators;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web {
    public sealed partial class AdditionalViewControlsProviderAspNetModule : AdditionalViewControlsProviderModuleBase {
        public AdditionalViewControlsProviderAspNetModule() {
            InitializeComponent();
        }

        protected override void OnRuleAdded(AdditionalViewControlsRuleNodeWrapper additionalViewControlsRuleNodeWrapper,
                                            AdditionalViewControlsAttribute additionalViewControlsAttribute) {
            base.OnRuleAdded(additionalViewControlsRuleNodeWrapper, additionalViewControlsAttribute);
            additionalViewControlsRuleNodeWrapper.ControlType = additionalViewControlsAttribute.ControlType ??
                                                                typeof (HintPanel);
            additionalViewControlsRuleNodeWrapper.DecoratorType = additionalViewControlsAttribute.DecoratorType ??
                                                                  typeof (WebHintPanelDecorator);
        }
    }
}