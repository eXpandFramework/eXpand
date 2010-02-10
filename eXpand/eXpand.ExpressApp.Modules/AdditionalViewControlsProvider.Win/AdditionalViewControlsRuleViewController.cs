using System.Windows.Forms;
using eXpand.ExpressApp.RuleModeller;
using ListView = DevExpress.ExpressApp.ListView;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win
{
    public class AdditionalViewControlsRuleViewController : Controllers.AdditionalViewControlsRuleViewController
    {
        protected override void AddControl(object viewSiteControl, object control, AdditionalViewControlsRuleInfo info, AdditionalViewControlsProviderCalculator calculator, ExecutionReason reason) {
            if (reason == ExecutionReason.TemplateViewChanged || reason == ExecutionReason.CurrentObjectChanged || reason == ExecutionReason.ObjectChanged)
            {
                var value = (Control)control;
                value.Dock = info.Rule.AdditionalViewControlsProviderPosition == AdditionalViewControlsProviderPosition.Bottom ? DockStyle.Bottom : DockStyle.Top;
                Control.ControlCollection collection = ((Control)viewSiteControl).Controls;
                var getControl = (Control) GetControl(collection, control, calculator, info);
                collection.Add(getControl);
            }
        }
    }
}


