using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Scheduler.Web;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxScheduler;
using Xpand.ExpressApp.Dashboard.Core.Dashboard;

namespace Xpand.ExpressApp.Dashboard.Web {
    public sealed class WebDashboardRefreshController : ViewController<DashboardView> {

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            foreach (var result in View.Items.OfType<DashboardViewItem>()) {
                result.ControlCreated += (sender, e) => {

                        DashboardViewItem viewItem = (DashboardViewItem)sender;
                        var listView = viewItem.InnerView as ListView;
                        if (listView != null) {
                            var editor = listView.Editor as ASPxGridListEditor;
                            if (editor != null) {
                                editor.ControlsCreated += editor_ControlsCreated;
                            }

                            var schedulerEditor = listView.Editor as ASPxSchedulerListEditor;
                            if (schedulerEditor != null) {
                                schedulerEditor.ResourceDataSourceCreating += schedulerEditor_ResourceDataSourceCreating;
                            }
                        }

                    };
            }

        }

        void schedulerEditor_ResourceDataSourceCreating(object sender, DevExpress.ExpressApp.Scheduler.ResourceDataSourceCreatingEventArgs e) {
            foreach (var result in View.Items.OfType<DashboardViewItem>()) {
                ListView listView = result.InnerView as ListView;
                if (listView != null && listView.Editor == sender) {
                    var model = result.Model as IModelDashboardViewItemEx;
                    if (model != null && model.Filter != null) {
                        var filterView = GetViewById(model.Filter.DataSourceView.Id);
                        if (filterView != null && filterView.ObjectTypeInfo.Type == e.ResourceType) {
                            var criteria = new InOperator(filterView.ObjectTypeInfo.KeyMember.Name,
                                                          filterView.SelectedObjects
                                                          .Cast<object>()
                                                          .Select(o => ObjectSpace.GetKeyValue(o)));

                            e.DataSource = new WebDataSource(ObjectSpace,
                                XafTypesInfo.Instance.FindTypeInfo(e.ResourceType),
                                ObjectSpace.CreateCollection(e.ResourceType, criteria));
                            e.Handled = true;
                            return;
                        }
                    }


                }
            }
        }


        private ListView GetViewById(string id) {
            return View.Items.OfType<DashboardViewItem>().Select(vi => vi.InnerView).FirstOrDefault(v => v.Id == id) as ListView;
        }

        void editor_ControlsCreated(object sender, EventArgs e) {
            ASPxGridListEditor editor = (ASPxGridListEditor)sender;
            editor.Grid.ClientSideEvents.Init = string.Format(
                CultureInfo.InvariantCulture,
                "function(s,e) {{s.SelectionChanged.AddHandler(function() {{{0}}}); }}", CallbackManager.GetScript(GetType().Name, "'RefreshCallback'"));
        }


        private XafCallbackManager CallbackManager {
            get { return ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager; }
        }

    }
}
