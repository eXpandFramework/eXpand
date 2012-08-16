using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.Web.ASPxCallbackPanel;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxSplitter;



namespace Xpand.ExpressApp.Web.Layout {
    public class XpandLayoutManager : WebLayoutManager {
        ViewItemsCollection viewItems;

        public override object LayoutControls(IModelNode layoutInfo, ViewItemsCollection detailViewItems) {
            var splitLayout = layoutInfo as IModelSplitLayout;
            if (splitLayout != null && detailViewItems.Count > 1) {
                viewItems = detailViewItems;
                ASPxSplitter splitter = CreateAsPxSplitter(splitLayout);
                SplitterPane listPane = splitter.Panes.Add();
                listPane.Name = "listPane";
                var listControl = (Control)detailViewItems[0].Control;
                listControl.ClientIDMode = ClientIDMode.Predictable;
                listPane.Controls.Add(listControl);
                splitter.ClientSideEvents.Init =
                    "function (s,e) {s.AdjustControl(); s.GetMainElement().ClientControl = s;}";
                splitter.ShowCollapseBackwardButton = true;
                splitter.ShowCollapseForwardButton = true;

                var gridView = listControl as ASPxGridView;

                if (gridView != null) {
                    if (string.IsNullOrEmpty(gridView.ClientInstanceName))
                        gridView.ClientInstanceName = "gridViewInSplitter";
                    splitter.ClientSideEvents.PaneResized = PaneResized(gridView);
                }

                SplitterPane detailPane = splitter.Panes.Add();
                detailPane.ScrollBars = ScrollBars.Auto;
                var updatePanel = new ASPxCallbackPanel { ID = "DetailUpdatePanel", ClientIDMode = ClientIDMode.Static };
                updatePanel.ClientSideEvents.Init = "function (s,e) {s.GetMainElement().ClientControl = s;}";
                var detailControl = (Control)detailViewItems[1].Control;
                detailControl.ClientIDMode = ClientIDMode.Predictable;
                updatePanel.Controls.Add(detailControl);
                detailPane.Controls.Add(updatePanel);
                return splitter;
            }
            return base.LayoutControls(layoutInfo, detailViewItems);
        }

        string PaneResized(ASPxGridView gridView) {
            return string.Format(CultureInfo.InvariantCulture,
                                 "function (s,e) {{ if (e.pane.name==='listPane') {{ {0}.SetWidth(e.pane.GetClientWidth()); {0}.SetHeight(e.pane.GetClientHeight());}}}}",
                                 gridView.ClientInstanceName);
        }

        ASPxSplitter CreateAsPxSplitter(IModelSplitLayout splitLayout) {
            return new ASPxSplitter {
                ID = "MasterDetailSplitter",
                ClientIDMode = ClientIDMode.Static,
                Orientation =
                    (splitLayout.Direction == FlowDirection.Horizontal) ? Orientation.Horizontal : Orientation.Vertical
            };
        }

        public override void BreakLinksToControls() {
            base.BreakLinksToControls();
            if (viewItems != null) {
                foreach (ViewItem item in viewItems) {
                    item.BreakLinksToControl(false);
                    var frameContainer = item as IFrameContainer;
                    if (frameContainer != null)
                        frameContainer.Frame.View.BreakLinksToControls();
                }
            }
        }
    }
}