using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler.UI;
using EditorAliases = Xpand.ExpressApp.Scheduler.Reminders.EditorAliases;

namespace Xpand.ExpressApp.Scheduler.Win.PropertyEditors {
    [PropertyEditor(typeof(TimeSpan), EditorAliases.TimeBeforeStartEditorAlias, false)]
    public class DurationPropertyEditor : DXPropertyEditor {
        public DurationPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        protected override object CreateControlCore() {
            return new DurationEdit();
        }

        protected override RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemDuration();
        }

        protected override void OnAllowEditChanged() {
            base.OnAllowEditChanged();
            if (Control != null) {
                Control.Enabled = AllowEdit;
            }
        }
    }
}
