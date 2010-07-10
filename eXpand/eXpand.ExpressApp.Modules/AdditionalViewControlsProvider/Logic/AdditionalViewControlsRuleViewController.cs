using System;
using DevExpress.ExpressApp.Templates;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Model;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Conditional.Logic;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public abstract class AdditionalViewControlsRuleViewController :ConditionalLogicRuleViewController<IAdditionalViewControlsRule> {
        
        public override void ExecuteRule(LogicRuleInfo<IAdditionalViewControlsRule> info, ExecutionContext executionContext) {
            if (Frame != null){
                var viewSiteTemplate = Frame.Template as IViewSiteTemplate;
                if (viewSiteTemplate == null)
                    return;
                object viewSiteControl = viewSiteTemplate.ViewSiteControl;
                if (viewSiteControl != null){
                    IAdditionalViewControlsRule additionalViewControlsRule = info.Rule;
                    var calculator = new AdditionalViewControlsProviderCalculator(additionalViewControlsRule, info.View.ObjectTypeInfo.Type);
                    Type controlType = calculator.ControlsRule.ControlType;
                    if (info.Active) {
                        object control = Activator.CreateInstance(controlType);
                        Activator.CreateInstance(calculator.ControlsRule.DecoratorType,new[] {info.View, control, info.Rule});
                        AddControl(viewSiteControl, control, info, calculator, executionContext);
                    }
                }
            }

        }
        protected override IModelGroupContexts GetModelGroupContexts(string executionContextGroup){
            return ((IModelApplicationAdditionalViewControls)Application.Model).AdditionalViewControls.GroupContexts;
        }




        protected abstract void AddControl(object viewSiteControl, object control, LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator, ExecutionContext context);
    }
}