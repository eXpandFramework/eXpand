using System;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.Web.ASPxCallbackPanel;
using DevExpress.Web.ASPxGlobalEvents;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Logic {
    public interface ISupportAdditionalViewControls {
        ASPxCallbackPanel TopPanel { get; }
        ASPxCallbackPanel BottomPanel { get; }
        ASPxGlobalEvents AsPxGlobalEvents { get; }
    }

    public class AdditionalViewControlsRuleViewController : AdditionalViewControlsProvider.Logic.AdditionalViewControlsRuleViewController {
        protected override Type GetControlType(){
            return typeof (Control);
        }

        protected override void AddControl(object control, object controls, LogicRuleInfo info) {
            base.AddControl(control, controls, info);
            var rule = (IAdditionalViewControlsRule) info.Rule;
            if (rule.Position != Position.DetailViewItem) {
                var supportAdditionalViewControls = Frame.Template as ISupportAdditionalViewControls;
                if (supportAdditionalViewControls != null) {
                    var asPxCallbackPanel = supportAdditionalViewControls.TopPanel;
                    if (rule.Position == Position.Bottom)
                        asPxCallbackPanel = supportAdditionalViewControls.BottomPanel;
                    var control1 = ((Control)control);
                    control1.Visible = info.Active;
                    asPxCallbackPanel.Controls.Add(control1);
                    ModifyClientEvent(supportAdditionalViewControls.AsPxGlobalEvents, info);
                }
            }

        }
        protected override void OnActivated() {
            base.OnActivated();
            if (HasRules && View is DetailView) {
                ResetInfoToLayoutMap();
                var detailView = ((DetailView)View);
                var winLayoutManager = ((WebLayoutManager)detailView.LayoutManager);
                winLayoutManager.ItemCreated += OnItemCreated;
            }
        }

        void OnItemCreated(object sender, ItemCreatedEventArgs itemCreatedEventArgs) {
            FillInfoToLayoutMap(itemCreatedEventArgs.ViewItem, itemCreatedEventArgs.ModelLayoutElement, itemCreatedEventArgs.ViewItem);
        }


        void ModifyClientEvent(ASPxGlobalEvents asPxGlobalEvents, LogicRuleInfo info) {
            asPxGlobalEvents.ClientSideEvents.EndCallback = info.Active
                                                                ? "function(s, e) { DXUpdateSplitterSize();DXMoveFooter(); if(s != TopCallBackPanel&&s != BottomCallBackPanel) {TopCallBackPanel.PerformCallback();BottomCallBackPanel.PerformCallback();};  }"
                                                                : "function(s, e) { DXUpdateSplitterSize();DXMoveFooter(); }";
        }
    }
}