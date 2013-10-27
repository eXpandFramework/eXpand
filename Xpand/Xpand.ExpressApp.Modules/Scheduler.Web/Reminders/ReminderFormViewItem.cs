using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Scheduler.Web.Reminders {
    
    public class ReminderFormViewItem:ControlDetailItem {
        protected internal ReminderFormViewItem(string controlTypeName, string id, string caption, Type objectType) : base(controlTypeName, id, caption, objectType) {
        }

        public ReminderFormViewItem(string id, string caption, object control) : base(id, caption, control) {
        }

        public ReminderFormViewItem(string id, object control) : base(id, control) {
        }

        public ReminderFormViewItem(IModelControlDetailItem model, Type objectType) : base(model, objectType) {
        }

        protected override object CreateControlCore() {
            return new DevExpress.Web.ASPxScheduler.Forms.Internal.ReminderForm();
        }
    }
}
