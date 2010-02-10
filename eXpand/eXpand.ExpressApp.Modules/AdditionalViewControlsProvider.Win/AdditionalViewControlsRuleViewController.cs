using System;
using System.Windows.Forms;
using System.Linq;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win
{
    public class AdditionalViewControlsRuleViewController : Controllers.AdditionalViewControlsRuleViewController
    {
        protected override void AddControl(object viewSiteControl, object control, AdditionalViewControlsRuleInfo info, AdditionalViewControlsProviderCalculator calculator) {
            var value = (Control)control;
            value.Dock = info.Rule.AdditionalViewControlsProviderPosition == AdditionalViewControlsProviderPosition.Bottom ? DockStyle.Bottom : DockStyle.Top;
            Control.ControlCollection collection = ((Control)viewSiteControl).Controls;
            Control firstOrDefault =collection.OfType<Control>().Where(control1 => control1.GetType().Equals(control.GetType())).FirstOrDefault() ??value;
            Activator.CreateInstance(calculator.ControlsRule.DecoratorType, new[] { info.View.CurrentObject, firstOrDefault, info.Rule });
            collection.Add(firstOrDefault);
        }
    }
}


