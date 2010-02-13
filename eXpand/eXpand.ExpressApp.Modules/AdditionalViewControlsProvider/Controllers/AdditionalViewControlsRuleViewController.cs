using System;
using System.Collections;
using System.Linq;
using DevExpress.ExpressApp.Templates;
using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers;
using eXpand.ExpressApp.RuleModeller;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Controllers {
    public abstract class AdditionalViewControlsRuleViewController :
        ModelRuleViewController
            <AdditionalViewControlsAttribute, AdditionalViewControlsRuleNodeWrapper, AdditionalViewControlsRuleInfo,
            AdditionalViewControlsRule> {
        public override void ExecuteRule(AdditionalViewControlsRuleInfo info, ExecutionReason executionReason) {
            if (info.Active ){
                AdditionalViewControlsRule additionalViewControlsRule = info.Rule;
                var calculator = new AdditionalViewControlsProviderCalculator(additionalViewControlsRule,info.View.ObjectTypeInfo.Type);
                object control = Activator.CreateInstance(calculator.ControlsRule.ControlType);
                AddControl(control,info,calculator,executionReason);
            }
        }

        protected object GetControl(IEnumerable collection, object control,
                                    AdditionalViewControlsProviderCalculator calculator,
                                    AdditionalViewControlsRuleInfo additionalViewControlsRule)
        {

            object o;
            if (additionalViewControlsRule.Rule.UseSameIfFound)
                o = collection.OfType<object>().Where(control1 => control1.GetType().Equals(control.GetType())).FirstOrDefault() ?? control;
            else
                o = control;
            Activator.CreateInstance(calculator.ControlsRule.DecoratorType, new[] { additionalViewControlsRule.View, o, additionalViewControlsRule.Rule });
            return o;
        }

        protected void AddControl(object control, AdditionalViewControlsRuleInfo additionalViewControlsRule,AdditionalViewControlsProviderCalculator calculator,ExecutionReason reason) {
            if (Frame != null) {
                var viewSiteTemplate = Frame.Template as IViewSiteTemplate;
                if (viewSiteTemplate == null)
                    return;
                object viewSiteControl = viewSiteTemplate.ViewSiteControl;
                if (viewSiteControl != null)
                    AddControl(viewSiteControl, control, additionalViewControlsRule,calculator,reason);
            }
        }

        protected abstract void AddControl(object viewSiteControl, object control, AdditionalViewControlsRuleInfo additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator, ExecutionReason reason);
    }
}