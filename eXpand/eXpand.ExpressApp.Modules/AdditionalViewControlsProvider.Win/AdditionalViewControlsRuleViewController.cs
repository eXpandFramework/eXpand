using System.Windows.Forms;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win
{
    public class AdditionalViewControlsRuleViewController : Logic.AdditionalViewControlsRuleViewController
    {
        protected override void AddControl(object viewSiteControl, object control, LogicRuleInfo<IAdditionalViewControlsRule> info, AdditionalViewControlsProviderCalculator calculator, ExecutionReason reason) {
            if (reason == ExecutionReason.TemplateViewChanged || reason == ExecutionReason.CurrentObjectChanged || reason == ExecutionReason.ObjectChanged){
                var value = (Control)control;
                value.Dock = info.Rule.AdditionalViewControlsProviderPosition == AdditionalViewControlsProviderPosition.Bottom ? DockStyle.Bottom : DockStyle.Top;
                Control.ControlCollection collection = ((Control)viewSiteControl).Controls;
                var getControl = (Control) GetControl(collection, control, calculator, info);
                collection.Add(getControl);
            }
        }
    }
}


