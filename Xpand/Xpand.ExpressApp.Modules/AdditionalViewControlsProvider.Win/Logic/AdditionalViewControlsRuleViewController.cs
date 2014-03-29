using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Layout;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Logic {
    public class AdditionalViewControlsRuleViewController : AdditionalViewControlsProvider.Logic.AdditionalViewControlsRuleViewController {
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.ViewChanged += FrameOnViewChanged;
        }

        protected override Type GetControlType(){
            return typeof (Control);
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (HasRules && View is DetailView) {
                ResetInfoToLayoutMap();
                var detailView = ((DetailView)View);
                var winLayoutManager = ((WinLayoutManager)detailView.LayoutManager);
                winLayoutManager.ItemCreated += OnItemCreated;

            }
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (HasRules && View is DetailView) {
                var detailView = ((DetailView)View);
                var winLayoutManager = ((WinLayoutManager)detailView.LayoutManager);
                winLayoutManager.Container.DefaultLayoutLoading += Container_DefaultLayoutLoading;
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (HasRules && View is DetailView) {
                ResetInfoToLayoutMap();
                var winLayoutManager = ((WinLayoutManager)((DetailView)View).LayoutManager);
                winLayoutManager.ItemCreated -= OnItemCreated;
                winLayoutManager.Container.DefaultLayoutLoading -= Container_DefaultLayoutLoading;
            }
        }

        void Container_DefaultLayoutLoading(object sender, EventArgs e) {
            ResetInfoToLayoutMap();
        }


        void OnItemCreated(object sender, ItemCreatedEventArgs itemCreatedEventArgs) {
            FillInfoToLayoutMap(itemCreatedEventArgs.ViewItem, itemCreatedEventArgs.ModelLayoutElement, itemCreatedEventArgs.Item);
        }

        void FrameOnViewChanged(object sender, ViewChangedEventArgs viewChangedEventArgs) {
            if (Frame.View != null) {
                if (Frame.View.IsControlCreated)
                    BringViewControlToFront(Frame.View.Control as Control);
                else
                    Frame.View.ControlsCreated += View_ControlsCreated;
            }
        }

        void View_ControlsCreated(object sender, EventArgs e) {
            Frame.View.ControlsCreated -= View_ControlsCreated;
            BringViewControlToFront(Frame.View.Control as Control);
        }

        void BringViewControlToFront(Control control) {
            if (control != null && control.Parent != null)
                control.BringToFront();
            else if (control != null)
                control.ParentChanged += control_ParentChanged;
        }

        protected override void InitializeControl(object control, IAdditionalViewControlsRule rule,
                                             AdditionalViewControlsProviderCalculator additionalViewControlsProviderCalculator,
                                             ExecutionContext executionContext) {
            base.InitializeControl(control, rule, additionalViewControlsProviderCalculator, executionContext);
            var value = (Control)control;
            if (rule.Position != Position.DetailViewItem) {
                if (rule.Position == Position.Bottom)
                    value.Dock = DockStyle.Bottom;
                else if (rule.Position == Position.Top)
                    value.Dock = DockStyle.Top;
            } else {
                value.Dock = DockStyle.Fill;
            }
        }

        void control_ParentChanged(object sender, EventArgs e) {
            var control = ((Control)sender);
            control.ParentChanged -= control_ParentChanged;
            control.BringToFront();
        }

    }
}