using System;
using DevExpress.ExpressApp.Templates;
using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers;
using eXpand.ExpressApp.RuleModeller;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Controllers {
    public abstract class AdditionalViewControlsRuleViewController :
        ModelRuleViewController
            <AdditionalViewControlsAttribute, AdditionalViewControlsRuleNodeWrapper, AdditionalViewControlsRuleInfo,
            AdditionalViewControlsRule> {
        public override void ExecuteRule(AdditionalViewControlsRuleInfo info, ExecutionReason executionReason) {
            if (info.Active && (executionReason == ExecutionReason.ViewControlAdding )){
                AdditionalViewControlsRule additionalViewControlsRule = info.Rule;
                var calculator = new AdditionalViewControlsProviderCalculator(additionalViewControlsRule);
                object control = Activator.CreateInstance(calculator.ControlsRule.ControlType);
                AddControl(control,info,calculator);
            }
        }
        protected void AddControl(object control, AdditionalViewControlsRuleInfo additionalViewControlsRule,AdditionalViewControlsProviderCalculator calculator) {
            if (Frame != null) {
                var viewSiteTemplate = Frame.Template as IViewSiteTemplate;
                if (viewSiteTemplate == null)
                    return;
                object viewSiteControl = viewSiteTemplate.ViewSiteControl;
                if (viewSiteControl != null)
                    AddControl(viewSiteControl, control, additionalViewControlsRule,calculator);
            }
        }

        protected abstract void AddControl(object viewSiteControl, object control, AdditionalViewControlsRuleInfo additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator);
    }
}