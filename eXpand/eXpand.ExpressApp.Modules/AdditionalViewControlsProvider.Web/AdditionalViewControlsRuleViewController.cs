using System;
using System.Linq;
using System.Web.UI;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web {
    public class AdditionalViewControlsRuleViewController : Logic.AdditionalViewControlsRuleViewController{
        protected override void RemoveControl(object viewSiteControl, Type controlType) {
            ControlCollection collection = ((Control) viewSiteControl).Controls;
            var getControl = collection.OfType<object>().Where(control1 => control1.GetType().Equals(controlType)).FirstOrDefault();
            collection.Remove((Control)getControl);
        }

        protected override object AddControl(object viewSiteControl, object control, LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator, ExecutionContext context) {
            ControlCollection collection = ((Control)viewSiteControl).Controls;
            object o = GetControl(collection, control, calculator, additionalViewControlsRule);
            ((Control) o).Visible = true;
            if (additionalViewControlsRule.Rule.AdditionalViewControlsProviderPosition == AdditionalViewControlsProviderPosition.Top)
                collection.AddAt(0, (Control)o);
            else
                collection.Add((Control)o);
            return o;
        }
    }
}