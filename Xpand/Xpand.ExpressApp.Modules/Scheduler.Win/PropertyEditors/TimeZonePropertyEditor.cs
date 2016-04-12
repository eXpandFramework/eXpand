using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraScheduler.UI;

namespace Xpand.ExpressApp.Scheduler.Win.PropertyEditors {
    [PropertyEditor(typeof(string),false)]
    public class TimeZoneEditPropertyEditor : WinPropertyEditor {
        public TimeZoneEditPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
        }

        protected override object CreateControlCore() {
            return new TimeZoneEdit();
        }
    }
}
