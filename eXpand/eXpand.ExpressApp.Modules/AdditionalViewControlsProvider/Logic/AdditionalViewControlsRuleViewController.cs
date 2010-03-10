using System;
using System.Collections;
using System.Linq;
using DevExpress.ExpressApp.Templates;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public abstract class AdditionalViewControlsRuleViewController :ConditionalLogicRuleViewController<IAdditionalViewControlsRule> {
        
        public override void ExecuteRule(LogicRuleInfo<IAdditionalViewControlsRule> logicRuleInfo, ExecutionContext executionContext) {
            if (logicRuleInfo.Active)
            {
                IAdditionalViewControlsRule additionalViewControlsRule = logicRuleInfo.Rule;
                var calculator = new AdditionalViewControlsProviderCalculator(additionalViewControlsRule, logicRuleInfo.View.ObjectTypeInfo.Type);
                object control = Activator.CreateInstance(calculator.ControlsRule.ControlType);
                AddControl(control, logicRuleInfo, calculator, executionContext);
            }
        }

        protected object GetControl(IEnumerable collection, object control,
                                    AdditionalViewControlsProviderCalculator calculator,
                                    LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule)
        {

            object o;
            if (additionalViewControlsRule.Rule.UseSameIfFound)
                o = collection.OfType<object>().Where(control1 => control1.GetType().Equals(control.GetType())).FirstOrDefault() ?? control;
            else
                o = control;
            Activator.CreateInstance(calculator.ControlsRule.DecoratorType, new[] { additionalViewControlsRule.View, o, additionalViewControlsRule.Rule });
            return o;
        }

        protected void AddControl(object control, LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule,AdditionalViewControlsProviderCalculator calculator,ExecutionContext context) {
            if (Frame != null) {
                var viewSiteTemplate = Frame.Template as IViewSiteTemplate;
                if (viewSiteTemplate == null)
                    return;
                object viewSiteControl = viewSiteTemplate.ViewSiteControl;
                if (viewSiteControl != null)
                    AddControl(viewSiteControl, control, additionalViewControlsRule,calculator,context);
            }
        }

        protected abstract void AddControl(object viewSiteControl, object control, LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator, ExecutionContext context);
    }
}