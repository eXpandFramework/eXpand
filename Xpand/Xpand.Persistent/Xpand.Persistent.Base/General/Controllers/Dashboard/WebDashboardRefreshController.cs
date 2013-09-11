using System;
using System.Globalization;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;

namespace Xpand.Persistent.Base.General.Controllers.Dashboard {
    public sealed class WebDashboardRefreshController : ViewController<DashboardView> {

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (XpandModuleBase.IsHosted) {
                foreach (var result in View.Items.OfType<DashboardViewItem>()) {
                    result.ControlCreated += ResultOnControlCreated;
                }
            }
        }

        void ResultOnControlCreated(object sender, EventArgs eventArgs) {
            var viewItem = (DashboardViewItem)sender;
            viewItem.ControlCreated-=ResultOnControlCreated;
            var listView = viewItem.InnerView as ListView;
            if (listView != null) {
                var editor = listView.Editor as ASPxGridListEditor;
                if (editor != null) {
                    editor.ControlsCreated += editor_ControlsCreated;
                }
            }
        }

        void editor_ControlsCreated(object sender, EventArgs e) {
            var editor = (ASPxGridListEditor)sender;
            editor.ControlsCreated-=editor_ControlsCreated;
            editor.Grid.ClientSideEvents.Init = string.Format(
                CultureInfo.InvariantCulture,
                "function(s,e) {{s.selectedRowCount = s.GetSelectedKeysOnPage().length; s.SelectionChanged.AddHandler(function() {{if (s.selectedRowCount != s.GetSelectedKeysOnPage().length) {{s.selectedRowCount = s.GetSelectedKeysOnPage().length; {0} }}; s.firstSelectionChangedAfterInit = false;}}); }}", CallbackManager.GetScript(GetType().Name, "'RefreshCallback'"));
        }


        private XafCallbackManager CallbackManager {
            get { return ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager; }
        }

    }
}
