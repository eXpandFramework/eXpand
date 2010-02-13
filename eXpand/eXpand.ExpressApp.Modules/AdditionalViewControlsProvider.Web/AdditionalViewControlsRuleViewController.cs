using System.Web.UI;
using eXpand.ExpressApp.RuleModeller;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web {
    public class AdditionalViewControlsRuleViewController : Controllers.AdditionalViewControlsRuleViewController{
        protected override void AddControl(object viewSiteControl, object control, AdditionalViewControlsRuleInfo additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator, ExecutionReason reason) {
            if (reason==ExecutionReason.TemplateViewChanged) {
                ControlCollection collection = ((Control)viewSiteControl).Controls;
                object o = GetControl(collection, control, calculator, additionalViewControlsRule);
                ((Control) o).Visible = true;
                if (additionalViewControlsRule.Rule.AdditionalViewControlsProviderPosition == AdditionalViewControlsProviderPosition.Top)
                    collection.AddAt(0, (Control)o);
                else
                    collection.Add((Control)o);
            }
        }
    }
}