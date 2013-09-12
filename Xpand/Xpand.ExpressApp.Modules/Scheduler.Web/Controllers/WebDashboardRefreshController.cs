using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Scheduler.Web;
using DevExpress.ExpressApp.Web.Editors;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.Scheduler.Web.Controllers {
    public sealed class WebDashboardRefreshController : ViewController<DashboardView> {

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            foreach (var result in View.Items.OfType<DashboardViewItem>()) {
                result.ControlCreated += ResultOnControlCreated;
            }
        }

        void ResultOnControlCreated(object sender, EventArgs eventArgs) {
            var viewItem = (DashboardViewItem)sender;
            viewItem.ControlCreated-=ResultOnControlCreated;
            var listView = viewItem.InnerView as ListView;
            if (listView != null) {
                var schedulerEditor = listView.Editor as ASPxSchedulerListEditor;
                if (schedulerEditor != null) {
                    schedulerEditor.ResourceDataSourceCreating += schedulerEditor_ResourceDataSourceCreating;
                }
            }
        }

        void schedulerEditor_ResourceDataSourceCreating(object sender, DevExpress.ExpressApp.Scheduler.ResourceDataSourceCreatingEventArgs e) {
            foreach (var result in View.Items.OfType<DashboardViewItem>()) {
                var listView = result.InnerView as ListView;
                if (listView != null && listView.Editor == sender) {
                    var model = result.Model as IModelDashboardViewItemEx;
                    if (model != null && model.Filter != null) {
                        var filterView = GetViewById(model.Filter.DataSourceView.Id);
                        if (filterView != null && filterView.ObjectTypeInfo.Type == e.ResourceType) {
                            e.Handled = true;
                            e.DataSource = WebDataSource(e.ResourceType, filterView);
                            return;
                        }
                    }
                }
            }
        }

        WebDataSource WebDataSource(Type resourceType, ListView filterView) {
            var criteria = new InOperator(filterView.ObjectTypeInfo.KeyMember.Name,
                                          filterView.SelectedObjects.Cast<object>().Select(o => ObjectSpace.GetKeyValue(o)));
            return new WebDataSource(ObjectSpace, Application.TypesInfo.FindTypeInfo(resourceType), ObjectSpace.CreateCollection(resourceType, criteria));
        }


        private ListView GetViewById(string id) {
            return View.Items.OfType<DashboardViewItem>().Select(vi => vi.InnerView).FirstOrDefault(v => v != null && v.Id == id) as ListView;
        }

    }
}