using System;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.ExpressApp.ExcelImporter.Win.BusinessObjects{
    public class ImportNotification : XpandCustomObject, ISupportNotifications{
        private DateTime? _alarmTime;

        private bool _isPostponed;

        private string _notificationMessage;

        public ImportNotification(Session session) : base(session){
        }

        public DateTime? AlarmTime{
            get => _alarmTime;
            set => SetPropertyValue(nameof(AlarmTime), ref _alarmTime, value);
        }

        object ISupportNotifications.UniqueId => Oid;

        public string NotificationMessage{
            get => _notificationMessage;
            set => SetPropertyValue(nameof(NotificationMessage), ref _notificationMessage, value);
        }

        public bool IsPostponed{
            get => _isPostponed;
            set => SetPropertyValue(nameof(IsPostponed), ref _isPostponed, value);
        }
    }
}