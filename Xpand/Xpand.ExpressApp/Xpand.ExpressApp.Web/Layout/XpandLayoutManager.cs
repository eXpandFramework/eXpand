﻿using System;
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



namespace Xpand.ExpressApp.Web.Layout {
    public class XpandLayoutManager : WebLayoutManager {
        ViewItemsCollection _detailViewItems;

        public override object LayoutControls(IModelNode layoutInfo, ViewItemsCollection detailViewItems) {
            var splitLayout = layoutInfo as IModelSplitLayout;
            if (IsMasterDetail(layoutInfo, detailViewItems, splitLayout)) {
                _detailViewItems = detailViewItems;
                var gridView = (Control)detailViewItems[0].Control as ASPxGridView;
                if (gridView != null) {
                    var detailControl = (Control)detailViewItems[1].Control;
                    SetupViewItems(detailControl, gridView);
                    return LayoutControls(detailControl, gridView, splitLayout);
                }
                throw new NotImplementedException(detailViewItems[0].Control.ToString());
            }
            return base.LayoutControls(layoutInfo, detailViewItems);
        }

        bool IsMasterDetail(IModelNode layoutInfo, ViewItemsCollection detailViewItems, IModelSplitLayout splitLayout) {
            return splitLayout != null && detailViewItems.Count > 1 && ((IModelListView)layoutInfo.Parent).MasterDetailMode == MasterDetailMode.ListViewAndDetailView;
        }

        object LayoutControls(Control detailControl, ASPxGridView gridView, IModelSplitLayout splitLayout) {
            ASPxSplitter splitter = CreateAsPxSplitter(splitLayout, PaneResized(gridView));
            var listPane = CreateSplitterListPane(splitter);
            listPane.Controls.Add(gridView);

            var asPxCallbackPanel = CreateSplitterDetailPane(splitter);
            asPxCallbackPanel.Controls.Add(detailControl);
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
            detailPane.Controls.Add(updatePanel);
            return updatePanel;
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

        ASPxSplitter CreateAsPxSplitter(IModelSplitLayout splitLayout, string paneResize) {
            var splitter = new ASPxSplitter {
                ID = "MasterDetailSplitter",
                Orientation = (splitLayout.Direction == FlowDirection.Horizontal) ? Orientation.Horizontal : Orientation.Vertical,
                ShowCollapseBackwardButton = true,
                ShowCollapseForwardButton = true,
                ClientInstanceName = "MasterDetailSplitter"
            };
            splitter.ClientSideEvents.Init = "function (s,e) {window.MasterDetailSplitter = s; s.AdjustControl(); s.GetMainElement().ClientControl = s;}";
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