using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Scheduler.Reminders {
    public struct EditorAliases {
        public const string TimeBeforeStartEditorAlias = "ReminderDurationPropertyEditor";
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SupportsReminderAttribute : Attribute {
        public SupportsReminderAttribute() {
            MemberName = "ReminderInfo";
        }

        public string Criteria { get; set; }
        public string MemberName { get; set; }
    }

    public class ReminderInfo : XpandCustomObject {

        private string _info;
        private TimeSpan _timeBeforeStart;
        private bool _hasReminder;

        public ReminderInfo(Session session)
            : base(session) {
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "Info") {
                HasReminder = !string.IsNullOrEmpty(Info);
                TimeBeforeStart=HasReminder?TimeSpan.Zero:TimeSpan.MinValue;
            }
        }

        [ImmediatePostData]
        public bool HasReminder {
            get { return _hasReminder; }
            set { SetPropertyValue("HasReminder", ref _hasReminder, value); }
        }

        [EditorAlias(EditorAliases.TimeBeforeStartEditorAlias)]
        public TimeSpan TimeBeforeStart {
            get { return _timeBeforeStart; }
            set { SetPropertyValue("TimeBeforeStart", ref _timeBeforeStart, value); }
        }

        [Size(SizeAttribute.Unlimited)]
//        [Browsable(false)]
        public string Info {
            get { return _info; }
            set { SetPropertyValue("Info", ref _info, value); }
        }
    }
}