using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.Web.ASPxCallbackPanel;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxSplitter;
using System.IO;
using System.Reflection;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using System.Collections.Generic;
using DevExpress.Web.ASPxClasses.Internal;
using System.Text;
using System.Linq;



namespace Xpand.ExpressApp.Web.Layout {
    public class XpandLayoutManager : WebLayoutManager {
        ViewItemsCollection _detailViewItems;

        public event EventHandler<MasterDetailLayoutEventArgs> MasterDetailLayout;


        public override object LayoutControls(IModelNode layoutInfo, ViewItemsCollection detailViewItems) {
            var splitLayout = layoutInfo as IModelSplitLayout;
            if (IsMasterDetail(layoutInfo, detailViewItems, splitLayout)) {
                _detailViewItems = detailViewItems;
                var gridView = (Control)detailViewItems[0].Control as ASPxGridView;
                if (gridView != null) {
                    var detailControl = (Control)detailViewItems[1].Control;
                    SetupViewItems(detailControl, gridView);
                    ASPxSplitter splitter = LayoutMasterDetail(detailControl, gridView, splitLayout);

                    RaiseMasterDetailLayout(new MasterDetailLayoutEventArgs() {
                        MasterViewItem = detailViewItems[0],
                        DetailViewItem = detailViewItems[1],
                        SplitterControl = splitter
                    });

                    return splitter;
                }
                throw new NotImplementedException(detailViewItems[0].Control.ToString());
            }
            return base.LayoutControls(layoutInfo, detailViewItems);
        }

        private void RaiseMasterDetailLayout(MasterDetailLayoutEventArgs args) {
            if (MasterDetailLayout != null) {
                MasterDetailLayout(this, args);
            }
        }

        bool IsMasterDetail(IModelNode layoutInfo, ViewItemsCollection detailViewItems, IModelSplitLayout splitLayout) {
            return splitLayout != null && detailViewItems.Count > 1 && ((IModelListView)layoutInfo.Parent).MasterDetailMode == MasterDetailMode.ListViewAndDetailView;
        }

        ASPxSplitter LayoutMasterDetail(Control detailControl, ASPxGridView gridView, IModelSplitLayout splitLayout) {
            ASPxSplitter splitter = CreateSplitter(splitLayout, PaneResized(gridView));
            var listPane = CreateSplitterListPane(splitter);
            listPane.Controls.Add(gridView);

            var callbackPanel = CreateSplitterDetailPane(splitter);
            callbackPanel.Controls.Add(detailControl);
            return splitter;
        }

        void SetupViewItems(Control detailControl, ASPxGridView gridView) {
            if (string.IsNullOrEmpty(gridView.ClientInstanceName))
                gridView.ClientInstanceName = "gridViewInSplitter";
        }

        private string GetAdjustSizeScript() {
            Type t = typeof(XpandLayoutManager);
            using (StreamReader reader = new StreamReader(t.Assembly.GetManifestResourceStream(
                string.Format(CultureInfo.InvariantCulture, "{0}.AdjustSize.js", t.Namespace)
                ))) {
                return reader.ReadToEnd();

            }
        }
        ASPxCallbackPanel CreateSplitterDetailPane(ASPxSplitter splitter) {
            SplitterPane detailPane = splitter.Panes.Add();
            detailPane.ScrollBars = ScrollBars.Auto;
            var updatePanel = new ASPxCallbackPanel { ID = "DetailUpdatePanel", ClientInstanceName = "DetailUpdatePanel" };
            updatePanel.ClientSideEvents.Init = GetAdjustSizeScript();
            updatePanel.ClientSideEvents.EndCallback = "function(s,e) {ProcessMarkup(s, true);}";
            updatePanel.CustomJSProperties += updatePanel_CustomJSProperties;
            detailPane.Controls.Add(updatePanel);
            return updatePanel;
        }

        void updatePanel_CustomJSProperties(object sender, DevExpress.Web.ASPxClasses.CustomJSPropertiesEventArgs e) {

            Page page = WebWindow.CurrentRequestPage;
            List<XafUpdatePanel> updatePanels = new List<XafUpdatePanel>();
            ICallbackManagerHolder callbackManagerHolder = page as ICallbackManagerHolder;
            FindUpdatePanels(page, updatePanels);
            StringBuilder controlNames = new StringBuilder();
            foreach (XafUpdatePanel panel in updatePanels) {
                if (!IsParentOf(panel, (Control)sender)) {
                    controlNames.Append(panel.ClientID);
                    controlNames.Append(";");
                    e.Properties["cp" + panel.ClientID] = RenderUtils.GetControlChildrenRenderResult(panel);
                }
            }

            e.Properties["cpControlsToUpdate"] = controlNames.ToString();

        }


        private static bool IsParentOf(Control parent, Control child) {
            for (Control c = child; c != null; c = c.Parent)
                if (c == parent)
                    return true;

            return false;
        }

        private bool ContainsActions(Control control) {
            return
                control != null && (
                control is DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder ||
                control is DevExpress.ExpressApp.Templates.IActionContainer ||
                control.Controls.Cast<Control>().Any(c => ContainsActions(c)));

        }

        private void FindUpdatePanels(Control sourceControl, List<XafUpdatePanel> updatePanels) {
            XafUpdatePanel updatePanel = sourceControl as XafUpdatePanel;
            if (updatePanel != null && updatePanel.UpdateAlways && !updatePanels.Contains(updatePanel) && ContainsActions(updatePanel)) {
                updatePanels.Add(updatePanel);
            }

            if (sourceControl != null) {
                foreach (Control currentControl in sourceControl.Controls) {
                    if (currentControl != null) {
                        FindUpdatePanels(currentControl, updatePanels);
                    }
                }
            }
        }


        SplitterPane CreateSplitterListPane(ASPxSplitter splitter) {
            SplitterPane listPane = splitter.Panes.Add();
            listPane.Name = "listPane";
            return listPane;
        }

        string PaneResized(ASPxGridView gridView) {
            return string.Format(CultureInfo.InvariantCulture,
                                 "function (s,e) {{ if (e.pane.name==='listPane') {{ {0}.SetWidth(e.pane.GetClientWidth()); {0}.SetHeight(e.pane.GetClientHeight());}}}}",
                                 gridView.ClientInstanceName);
        }

        ASPxSplitter CreateSplitter(IModelSplitLayout splitLayout, string paneResize) {
            var splitter = new ASPxSplitter {
                ID = "MasterDetailSplitter",
                Orientation = (splitLayout.Direction == FlowDirection.Horizontal) ? Orientation.Horizontal : Orientation.Vertical,
                ShowCollapseBackwardButton = true,
                ShowCollapseForwardButton = true
            };
            splitter.ClientSideEvents.Init = "function (s,e) { if (XpandHelper.IsRootSplitter(s)) { window.MasterDetailSplitter = s; } s.AdjustControl(); s.GetMainElement().ClientControl = s;}";
            splitter.ClientSideEvents.PaneResized = paneResize;
            return splitter;
        }

        public override void BreakLinksToControls() {
            base.BreakLinksToControls();
            if (_detailViewItems != null) {
                foreach (ViewItem item in _detailViewItems) {
                    item.BreakLinksToControl(false);
                    var frameContainer = item as IFrameContainer;
                    if (frameContainer != null)
                        frameContainer.Frame.View.BreakLinksToControls();
                }
            }
        }

        internal void UpdateItemsVisibility() {
            MethodInfo method = GetType().BaseType.GetMethod("UpdateItemsVisibility", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
                method.Invoke(this, null);
            else
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Method 'UpdateItemsVisibility' not found in '{0}'", GetType().BaseType));
        }
    }
}