using System;
using System.Collections;
using System.Linq;
using DevExpress.ExpressApp.Templates;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public abstract class AdditionalViewControlsRuleViewController :ConditionalLogicRuleViewController<IAdditionalViewControlsRule> {
        
        public override void ExecuteRule(LogicRuleInfo<IAdditionalViewControlsRule> logicRuleInfo, ExecutionContext executionContext) {
            if (Frame != null)
            {
                var viewSiteTemplate = Frame.Template as IViewSiteTemplate;
                if (viewSiteTemplate == null)
                    return;
                object viewSiteControl = viewSiteTemplate.ViewSiteControl;
                if (viewSiteControl != null)
                {
                    IAdditionalViewControlsRule additionalViewControlsRule = logicRuleInfo.Rule;
                    var calculator = new AdditionalViewControlsProviderCalculator(additionalViewControlsRule, logicRuleInfo.View.ObjectTypeInfo.Type);
                    if (logicRuleInfo.Active) {
                        object control = Activator.CreateInstance(calculator.ControlsRule.ControlType);
                        AddControl(viewSiteControl, control, logicRuleInfo, calculator, executionContext);
                    }
                    else {
                        RemoveControl(viewSiteControl, calculator.ControlsRule.ControlType);
                    }
                }
            }

        }

        protected abstract void RemoveControl(object viewSiteControl, Type controlType);


        protected object GetControl(IEnumerable collection, object control,
                                    AdditionalViewControlsProviderCalculator calculator,
                                    LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule){

            object o;
            if (additionalViewControlsRule.Rule.UseSameIfFound)
                o = collection.OfType<object>().Where(control1 => control1.GetType().Equals(control.GetType())).FirstOrDefault() ?? control;
            else 
                o = control;
            Activator.CreateInstance(calculator.ControlsRule.DecoratorType, new[] { additionalViewControlsRule.View, o, additionalViewControlsRule.Rule });
            return o;
        }


        protected abstract object AddControl(object viewSiteControl, object control, LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator, ExecutionContext context);
    }
}