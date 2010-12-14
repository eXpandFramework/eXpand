using System.Web.UI;
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
        protected override void AddControl(object control, object controls, LogicRuleInfo<IAdditionalViewControlsRule> info) {
            var supportAdditionalViewControls = Frame.Template as ISupportAdditionalViewControls;
            if (supportAdditionalViewControls!=null) {
                var asPxCallbackPanel = supportAdditionalViewControls.TopPanel;
                if (info.Rule.Position == Position.Bottom)
                    asPxCallbackPanel = supportAdditionalViewControls.BottomPanel;
                var control1 = ((Control)control);
                control1.Visible = info.Active;
                asPxCallbackPanel.Controls.Add(control1);
                ModifyClientEvent(supportAdditionalViewControls.AsPxGlobalEvents, info);
            }

        }

        void ModifyClientEvent(ASPxGlobalEvents asPxGlobalEvents, LogicRuleInfo<IAdditionalViewControlsRule> info) {
            if (info.Active)
                asPxGlobalEvents.ClientSideEvents.EndCallback =
                    "function(s, e) { DXUpdateSplitterSize();DXMoveFooter(); if(s != TopCallBackPanel&&s != BottomCallBackPanel) {TopCallBackPanel.PerformCallback();BottomCallBackPanel.PerformCallback();};  }";
            else {
                asPxGlobalEvents.ClientSideEvents.EndCallback =
                    "function(s, e) { DXUpdateSplitterSize();DXMoveFooter(); }";
            }
        }

    }
}