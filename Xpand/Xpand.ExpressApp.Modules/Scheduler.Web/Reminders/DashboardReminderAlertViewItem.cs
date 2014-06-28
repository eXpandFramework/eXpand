using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.Scheduler.Web.Reminders {
    public interface IModelDashboardReminderAlertViewItem : IModelDashboardViewItem {
        [Browsable(false)]
        new IModelView View { get; set; }
        [Browsable(false)]
        new string Criteria { get; set; }
        [Browsable(false)]
        [DefaultValue(ActionsToolbarVisibility.Hide)]
        new ActionsToolbarVisibility ActionsToolbarVisibility { get; set; }
        [DefaultValue(ViewItemVisibility.Show)]
        [Browsable(false)]
        ViewItemVisibility Visibility { get; set; }
        [Browsable(false)]
        MasterDetailMode? MasterDetailMode { get; set; }
    }
    public interface IModelDashboardReportViewItem : IModelDashboardReportViewItemBase {

    }

    [ViewItem(typeof(IModelDashboardReportViewItem))]
    public class DashboardReminderAlertViewItem :DashboardViewItem, IComplexViewItem {
        public DashboardReminderAlertViewItem(IModelDashboardViewItem model, Type objectType) : base(model, objectType) {
        }
        protected override object CreateControlCore() {
            return new ReminderForm();
        }

        void IComplexViewItem.Setup(IObjectSpace objectSpace, XafApplication application) {
            Setup(objectSpace, application);
        }
    }

    public class ReminderForm : DevExpress.Web.ASPxScheduler.Forms.Internal.ReminderForm {
        public override void DataBind() {
            throw new NotImplementedException();
        }
    }
}
