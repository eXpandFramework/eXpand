using System.Web.UI;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web {
    public class AdditionalViewControlsRuleViewController : Controllers.AdditionalViewControlsRuleViewController
    {
        protected override void AddControl(object viewSiteControl, object control, AdditionalViewControlsRuleInfo additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator) {
            ControlCollection collection = ((Control)viewSiteControl).Controls;
            if (additionalViewControlsRule.Rule.AdditionalViewControlsProviderPosition == AdditionalViewControlsProviderPosition.Top)
                collection.AddAt(0, (Control)control);
            else
                collection.Add((Control)control);
        }
    }
}