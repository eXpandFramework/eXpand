using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using EditorAliases = Xpand.ExpressApp.Scheduler.Reminders.EditorAliases;

namespace Xpand.ExpressApp.Scheduler.Web.Reminders {
    [PropertyEditor(typeof(TimeSpan), EditorAliases.TimeBeforeStartEditorAlias, false)]
    public class WebDurationPropertyEditor : ASPxPropertyEditor {
        ASPxComboBox _control;

        public WebDurationPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        protected override System.Web.UI.WebControls.WebControl CreateViewModeControlCore() {
            var control = CreateDurationEditor();
            control.Enabled = false;
            return control;
        }

        protected override System.Web.UI.WebControls.WebControl CreateEditModeControlCore() {
            _control = CreateDurationEditor();
            _control.ValueChanged += EditValueChangedHandler;
            return _control;
        }

        public override void BreakLinksToControl(bool unwireEventsOnly) {
            if (_control != null)
                _control.ValueChanged -= EditValueChangedHandler;
            base.BreakLinksToControl(unwireEventsOnly);
        }

        ASPxComboBox CreateDurationEditor() {
            var result = RenderHelper.CreateASPxComboBox();
            InitDurationCombo(result);
            return result;
        }
        protected override object GetControlValueCore() {
            var controlValueCore = base.GetControlValueCore();
            var valueCore = controlValueCore as string;
            if (valueCore != null)
                return TimeSpan.Parse(valueCore);
            return controlValueCore;
        }

        static void InitDurationCombo(ASPxComboBox cbSnooze) {
            cbSnooze.Items.Clear();
            TimeSpan[] snoozeTimeSpans = GetTimeSpans();
            int length = snoozeTimeSpans.Length;
            for (int i = 0; i < length; i++) {
                TimeSpan span = snoozeTimeSpans[i];
                cbSnooze.Items.Add(new ListEditItem(ConvertTimeSpanToString(span), span));
            }
            cbSnooze.SelectedIndex = 4;
        }

        static string ConvertTimeSpanToString(TimeSpan timeSpan) {
            if (timeSpan.Ticks < 0L) {
                string str = HumanReadableTimeSpanHelper.ToString(timeSpan);
                return string.Format(SchedulerLocalizer.GetString(SchedulerStringId.Format_TimeBeforeStart), str);
            }
            return HumanReadableTimeSpanHelper.ToString(timeSpan);
        }

        static TimeSpan[] GetTimeSpans() {
            var list = new List<TimeSpan>();
            int length = ReminderTimeSpans.BeforeStartTimeSpanValues.Length;
            for (int i = 0; i < length; i++) {
                list.Add(ReminderTimeSpans.BeforeStartTimeSpanValues[i]);
            }
            length = ReminderTimeSpans.ReminderTimeSpanValues.Length;
            for (int j = 0; j < length; j++) {
                list.Add(ReminderTimeSpans.ReminderTimeSpanValues[j]);
            }
            return list.ToArray();
        }
    }
}
