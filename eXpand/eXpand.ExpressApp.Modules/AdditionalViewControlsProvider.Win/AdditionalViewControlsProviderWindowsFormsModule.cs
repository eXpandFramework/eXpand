using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Decorators;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win
{
    public sealed partial class AdditionalViewControlsProviderWindowsFormsModule : AdditionalViewControlsProviderModuleBase
    {
        public AdditionalViewControlsProviderWindowsFormsModule()
        {
            InitializeComponent();
        }
        protected override void OnRuleAdded(AdditionalViewControlsRuleNodeWrapper additionalViewControlsRuleNodeWrapper, AdditionalViewControlsAttribute additionalViewControlsAttribute){
            base.OnRuleAdded(additionalViewControlsRuleNodeWrapper, additionalViewControlsAttribute);
            additionalViewControlsRuleNodeWrapper.ControlType = additionalViewControlsAttribute.ControlType ?? typeof(HintPanel);
            additionalViewControlsRuleNodeWrapper.DecoratorType = additionalViewControlsAttribute.DecoratorType ?? typeof(WinHintPanelDecorator);
        }
    }
}
