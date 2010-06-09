using System;
using System.Linq;
using System.Web.UI;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Logic {
    public class AdditionalViewControlsRuleViewController : AdditionalViewControlsProvider.Logic.AdditionalViewControlsRuleViewController
    {
        protected override void RemoveControl(object viewSiteControl, Type controlType)
        {
            ControlCollection collection = ((Control)viewSiteControl).Controls;
            var getControl = collection.OfType<object>().Where(control1 => control1.GetType().Equals(controlType)).FirstOrDefault();
            collection.Remove((Control)getControl);
        }

        protected override object AddControl(object viewSiteControl, object control, LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator, ExecutionContext context)
        {
            ControlCollection collection = ((Control)viewSiteControl).Controls;
            object o = GetControl(collection, control, calculator, additionalViewControlsRule);
            ((Control)o).Visible = true;
            switch (additionalViewControlsRule.Rule.Position) {
                case Position.Top:
                    collection.AddAt(0, (Control)o);
                    break;
                default:
                    collection.Add((Control)o);
                    break;
            }
            return o;
        }
    }
}