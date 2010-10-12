using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraLayout;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Editors;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;
using Xpand.ExpressApp.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Logic {
    public class AdditionalViewControlsRuleViewController : AdditionalViewControlsProvider.Logic.AdditionalViewControlsRuleViewController {
        Dictionary<string, BaseLayoutItem> _infoToLayoutMapCore;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.ViewChanged += FrameOnViewChanged;
        }
        protected override void OnActivated() {
            base.OnActivated();
            if (IsReady && View is DetailView) {
                ResetInfoToLayoutMap();
                var detailView = ((DetailView)View);
                var winLayoutManager = ((WinLayoutManager)detailView.LayoutManager);
                winLayoutManager.ItemCreated += OnItemCreated;

            }
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (IsReady && View is DetailView) {
                var detailView = ((DetailView)View);
                var winLayoutManager = ((WinLayoutManager)detailView.LayoutManager);
                winLayoutManager.Container.DefaultLayoutLoading -= Container_DefaultLayoutLoading;
            }
        }
        protected void FillInfoToLayoutMap(ViewItem detailViewItem, IModelDetailViewLayoutElement itemModel, BaseLayoutItem layoutItem) {
            if (detailViewItem is AdditionalViewControlsItem) {
                var id = ((AdditionalViewControlsItem)detailViewItem).Model.Rule.Id;
                if (RuleToLayoutMap.ContainsKey(id))
                    RuleToLayoutMap[id] = layoutItem;
                else
                    RuleToLayoutMap.Add(id, layoutItem);
            }

        }

        protected override void OnDeactivating() {
            base.OnDeactivating();
            if (IsReady && View is DetailView) {
                ResetInfoToLayoutMap();
                var winLayoutManager = ((WinLayoutManager)((DetailView)View).LayoutManager);
                winLayoutManager.ItemCreated -= OnItemCreated;
                winLayoutManager.Container.DefaultLayoutLoading -= Container_DefaultLayoutLoading;
            }
        }

        protected void ResetInfoToLayoutMap() {
            RuleToLayoutMap.Clear();
        }

        void Container_DefaultLayoutLoading(object sender, EventArgs e) {
            ResetInfoToLayoutMap();
        }

        protected Dictionary<string, BaseLayoutItem> RuleToLayoutMap {
            get { return _infoToLayoutMapCore ?? (_infoToLayoutMapCore = new Dictionary<string, BaseLayoutItem>()); }
        }

        void OnItemCreated(object sender, ItemCreatedEventArgs itemCreatedEventArgs) {
            FillInfoToLayoutMap(itemCreatedEventArgs.DetailViewItem, itemCreatedEventArgs.ModelLayoutElement, itemCreatedEventArgs.Item);
        }

        void FrameOnViewChanged(object sender, ViewChangedEventArgs viewChangedEventArgs) {
            if (Frame.View != null && Frame.View.IsControlCreated) {
                var control = Frame.View.Control as Control;
                if (control != null) {
                    control.BringToFront();
                }
            }
        }

        protected override void RemoveControl(IList controls, object firstOrDefault, LogicRuleInfo<IAdditionalViewControlsRule> info) {
            if (info.Rule.Position != Position.DetailViewItem)
                base.RemoveControl(controls, firstOrDefault, info);
        }
        protected override object GetControl(Type controlType, object o, LogicRuleInfo<IAdditionalViewControlsRule> info) {
            object control = base.GetControl(controlType, o, info);
            if (control is ISupportLayoutManager) {
                if (info.Rule.Position != Position.DetailViewItem)
                    throw new ArgumentException("Rule:" + info.Rule.Id + " position should be set to " + Position.DetailViewItem);
                ((ISupportLayoutManager)control).LayoutItem = RuleToLayoutMap[info.Rule.Id];
            }
            return control;
        }

        protected override void InitializeControl(object control, LogicRuleInfo<IAdditionalViewControlsRule> logicRuleInfo,
                                             AdditionalViewControlsProviderCalculator additionalViewControlsProviderCalculator,
                                             ExecutionContext executionContext) {
            base.InitializeControl(control, logicRuleInfo, additionalViewControlsProviderCalculator, executionContext);
            var value = (Control)control;
            //            value.Visible = true;
            if (logicRuleInfo.Rule.Position != Position.DetailViewItem) {
                if (logicRuleInfo.Rule.Position == Position.Bottom)
                    value.Dock = DockStyle.Bottom;
                else if (logicRuleInfo.Rule.Position == Position.Top)
                    value.Dock = DockStyle.Top;
            } else {
                value.Dock = DockStyle.Fill;
            }
        }

    }
}