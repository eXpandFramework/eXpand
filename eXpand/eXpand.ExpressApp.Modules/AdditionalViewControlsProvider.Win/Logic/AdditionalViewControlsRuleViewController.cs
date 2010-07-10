using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Logic {
    public class AdditionalViewControlsRuleViewController : AdditionalViewControlsProvider.Logic.AdditionalViewControlsRuleViewController {
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.ViewChanged+=FrameOnViewChanged;
        }

        void FrameOnViewChanged(object sender, ViewChangedEventArgs viewChangedEventArgs) {
            if (Frame.View != null) {
                try {
                    var control = Frame.View.Control as Control;
                    if (control != null) {
                        control.BringToFront();
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }

        protected override void AddControl(object viewSiteControl, object control,
                                             LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule,
                                             AdditionalViewControlsProviderCalculator calculator,
                                             ExecutionContext context) {
            var value = (Control) control;
            value.Visible = true;
            value.Dock = additionalViewControlsRule.Rule.Position == Position.Bottom ? DockStyle.Bottom : DockStyle.Top;
            Control.ControlCollection collection = ((Control) viewSiteControl).Controls;
            collection.Add(value);
        }

    }
}