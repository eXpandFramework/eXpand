using System;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.Quartz {
    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("SimpleTrigger")]
    public class XpandSimpleTrigger : XpandTrigger {
        public XpandSimpleTrigger(Session session)
            : base(session) {
        }
        
        private int _repeatCount;
        public int RepeatCount {
            get {
                return _repeatCount;
            }
            set {
                SetPropertyValue("RepeatCount", ref _repeatCount, value);
            }
        }
        private TimeSpan _repeatInterval;
        public TimeSpan RepeatInterval {
            get {
                return _repeatInterval;
            }
            set {
                SetPropertyValue("RepeatInterval", ref _repeatInterval, value);
            }
        }
        private int _timesTriggered;
        public int TimesTriggered {
            get {
                return _timesTriggered;
            }
            set {
                SetPropertyValue("TimesTriggered", ref _timesTriggered, value);
            }
        }
        private DateTime? _finalFireTimeUtc;
        public DateTime? FinalFireTimeUtc {
            get {
                return _finalFireTimeUtc;
            }
            set {
                SetPropertyValue("FinalFireTimeUtc", ref _finalFireTimeUtc, value);
            }
        }
        [ProvidedAssociation("XpandSimpleTrigger-JobDetails",RelationType.ManyToMany)]
        public XPCollection<XpandJobDetail> JobDetails {
            get {
                return GetCollection<XpandJobDetail>("JobDetails");
            }
        }
    }
}