using System;
using System.Linq;
using System.Windows.Forms;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Logic {
    public class AdditionalViewControlsRuleViewController : AdditionalViewControlsProvider.Logic.AdditionalViewControlsRuleViewController {
        protected override void RemoveControl(object viewSiteControl, Type controlType) {
            Control.ControlCollection collection = ((Control) viewSiteControl).Controls;
            object getControl =
                collection.OfType<object>().Where(control1 => control1.GetType().Equals(controlType)).FirstOrDefault();
            collection.Remove((Control) getControl);
        }

        protected override object AddControl(object viewSiteControl, object control,
                                             LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule,
                                             AdditionalViewControlsProviderCalculator calculator,
                                             ExecutionContext context) {
            var value = (Control) control;
            value.Dock = additionalViewControlsRule.Rule.Position == Position.Bottom ? DockStyle.Bottom : DockStyle.Top;
            Control.ControlCollection collection = ((Control) viewSiteControl).Controls;
            var getControl = (Control) GetControl(collection, control, calculator, additionalViewControlsRule);
            collection.Add(getControl);
            return getControl;
        }


    }
}