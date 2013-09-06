using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.Web.ASPxCallbackPanel;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxSplitter;
using System.Reflection;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using System.Collections.Generic;
using DevExpress.Web.ASPxClasses.Internal;
using System.Text;
using System.Linq;
using Xpand.ExpressApp.ListEditors;
using Xpand.ExpressApp.Web.ListEditors;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;


namespace Xpand.ExpressApp.Web.Layout {

    public interface IWebLayoutManager {
        event EventHandler<TemplateInstantiatedEventArgs> Instantiated;
    }

    public class XpandLayoutManager : WebLayoutManager, ILayoutManager, IWebLayoutManager {
        public event EventHandler<TemplateInstantiatedEventArgs> Instantiated;

        private static readonly List<Tuple<Type, Type>> listControlAdapters = new List<Tuple<Type, Type>>();
        private ViewItemsCollection _detailViewItems;

        protected virtual void OnInstantiated(TemplateInstantiatedEventArgs e) {
            var handler = Instantiated;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<MasterDetailLayoutEventArgs> MasterDetailLayout;



        static XpandLayoutManager() {
            RegisterListControlAdapter(typeof(ASPxGridView), typeof(GridViewListAdapter));
        }

        public static void RegisterListControlAdapter(Type controlType, Type adapterType) {
            Guard.ArgumentNotNull(controlType, "controlType");
            if (!typeof(IListControlAdapter).IsAssignableFrom(adapterType))
                throw new ArgumentException("Class must implement the IListControlAdapter interface", "adapterType");

            for (int i = 0; i < listControlAdapters.Count; i++) {
                if (listControlAdapters[i].Item1.IsAssignableFrom(controlType)) {
                    listControlAdapters[i] = new Tuple<Type, Type>(controlType, adapterType);
                    return;
                }
            }

            listControlAdapters.Add(new Tuple<Type, Type>(controlType, adapterType));
        }


        public override object LayoutControls(IModelNode layoutInfo, ViewItemsCollection detailViewItems) {
            var splitLayout = layoutInfo as IModelSplitLayout;
            if (IsMasterDetail(layoutInfo, detailViewItems, splitLayout)) {
                _detailViewItems = detailViewItems;

                IListControlAdapter adapter = GetListControlAdapter((Control)detailViewItems[0].Control);
                if (adapter != null) {
                    var detailControl = (Control)detailViewItems[1].Control;
                    ASPxSplitter splitter = LayoutMasterDetail(detailControl, adapter, splitLayout);
                    var viewItem = detailViewItems[0] as ListEditorViewItem;

                    if (viewItem != null) {
                        var listEditor = viewItem.ListEditor as IXpandListEditor;
                        if (listEditor != null) {
                            listEditor.ViewControlsCreated += (s, e) => SetSplitterInitClientEvent(splitter, e.IsRoot);
                        }
                    }

                    RaiseMasterDetailLayout(new MasterDetailLayoutEventArgs {
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

        protected override LayoutBaseTemplate CreateLayoutItemTemplate() {
            var layoutBaseTemplate = base.CreateLayoutItemTemplate();
            layoutBaseTemplate.Instantiated+=LayoutBaseTemplateOnInstantiated;
            return layoutBaseTemplate;
        }

        protected override LayoutBaseTemplate CreateLayoutGroupTemplate() {
            var layoutBaseTemplate = base.CreateLayoutGroupTemplate();
            layoutBaseTemplate.Instantiated += LayoutBaseTemplateOnInstantiated;
            return layoutBaseTemplate;
        }

        void LayoutBaseTemplateOnInstantiated(object sender, TemplateInstantiatedEventArgs templateInstantiatedEventArgs) {
            OnInstantiated(templateInstantiatedEventArgs);
        }

        private static IListControlAdapter GetListControlAdapter(Control control) {
            Guard.ArgumentNotNull(control, "control");

            Type t = listControlAdapters.Where(at => at.Item1.IsInstanceOfType(control)).Select(at => at.Item2).FirstOrDefault();
            if (t == null)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "No IListControlAdapter found for controlType {0}", control.GetType()));

            var adapter = (IListControlAdapter)Activator.CreateInstance(t);
            adapter.Control = control;
            return adapter;
        }
        private static void SetSplitterInitClientEvent(ASPxSplitter splitter, bool isRoot) {
            splitter.ClientSideEvents.Init = string.Format(CultureInfo.InvariantCulture,
                "function (s,e) {{ {0}  s.AdjustControl(); s.GetMainElement().ClientControl = s;}}", isRoot ? "window.MasterDetailSplitter = s;" : string.Empty);
        }

        private void RaiseMasterDetailLayout(MasterDetailLayoutEventArgs args) {
            if (MasterDetailLayout != null) {
                MasterDetailLayout(this, args);
            }
        }

        bool IsMasterDetail(IModelNode layoutInfo, ViewItemsCollection detailViewItems, IModelSplitLayout splitLayout) {
            return splitLayout != null && detailViewItems.Count > 1 && ((IModelListView)layoutInfo.Parent).MasterDetailMode == MasterDetailMode.ListViewAndDetailView;
        }

        ASPxSplitter LayoutMasterDetail(Control detailControl, IListControlAdapter adapter, IModelSplitLayout splitLayout) {
            ASPxSplitter splitter = CreateSplitter(splitLayout, adapter);
            var listPane = CreateSplitterListPane(splitter);
            listPane.Controls.Add(adapter.Control);

            var callbackPanel = CreateSplitterDetailPane(splitter);
            callbackPanel.Controls.Add(detailControl);
            return splitter;
        }


        private static string GetScript(string scriptName) {
            Type t = typeof(XpandLayoutManager);

            return
                t.Assembly.GetManifestResourceStream(string.Format(CultureInfo.InvariantCulture, "{0}.{1}.js", t.Namespace, scriptName)).ReadToEndAsString();
        }

        private static string GetAdjustSizeScript() {
            return GetScript("AdjustSize");
        }

        public static string GetXpandHelperScript() {
            return GetScript("XpandHelper");
        }

        ASPxCallbackPanel CreateSplitterDetailPane(ASPxSplitter splitter) {
            SplitterPane detailPane = splitter.Panes.Add();
            detailPane.ScrollBars = ScrollBars.Auto;
            var updatePanel = new ASPxCallbackPanel { ID = "DetailUpdatePanel", ClientInstanceName = "DetailUpdatePanel" };

            if (!(WebWindow.CurrentRequestWindow is PopupWindow))
                updatePanel.ClientSideEvents.Init = GetAdjustSizeScript();


            updatePanel.ClientSideEvents.EndCallback = "function(s,e) {ProcessMarkup(s, true);}";
            updatePanel.CustomJSProperties += updatePanel_CustomJSProperties;
            detailPane.Controls.Add(updatePanel);
            return updatePanel;
        }

        void updatePanel_CustomJSProperties(object sender, DevExpress.Web.ASPxClasses.CustomJSPropertiesEventArgs e) {

            Page page = WebWindow.CurrentRequestPage;
            var updatePanels = new List<XafUpdatePanel>();
            FindUpdatePanels(page, updatePanels);
            var controlNames = new StringBuilder();
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
                control.Controls.Cast<Control>().Any(ContainsActions));

        }

        private void FindUpdatePanels(Control sourceControl, List<XafUpdatePanel> updatePanels) {
            var updatePanel = sourceControl as XafUpdatePanel;
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

        private string GetPaneResizedEventScript(IListControlAdapter adapter) {
            return string.Format(CultureInfo.InvariantCulture,
                                 "function (s,e) {{ if (e.pane.name==='listPane') {{ {0};s.xpandInitialized  = true; }}}}",
                                  adapter.CreateSetBoundsScript("e.pane.GetClientWidth()", "e.pane.GetClientHeight()"));
        }

        private ASPxSplitter CreateSplitter(IModelSplitLayout splitLayout, IListControlAdapter adapter) {
            var splitter = new ASPxSplitter {
                ID = "MasterDetailSplitter",
                Orientation =
                    (splitLayout.Direction == FlowDirection.Horizontal)
                        ? Orientation.Horizontal
                        : Orientation.Vertical,
                ShowCollapseBackwardButton = true,
                ShowCollapseForwardButton = true
            };

            splitter.ClientSideEvents.PaneResized = GetPaneResizedEventScript(adapter);
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
            var baseType = GetType().BaseType;
            if (baseType != null) {
                MethodInfo method = baseType.GetMethod("UpdateItemsVisibility", BindingFlags.Instance | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(this, null);
                else
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Method 'UpdateItemsVisibility' not found in '{0}'", baseType));
            }
        }
    }

}