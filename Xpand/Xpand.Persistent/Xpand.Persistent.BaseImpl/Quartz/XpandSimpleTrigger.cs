using System;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.BaseImpl.Quartz {
    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("SimpleTrigger")]
    public class XpandSimpleTrigger : XpandTrigger {
        public XpandSimpleTrigger(Session session)
            : base(session) {
        }
        private SimpleTriggerMisfireInstruction _misfireInstruction;
        [Tooltip("Get or set the instruction the IScheduler should be given for handling misfire situations for this Trigger- the concrete Trigger type that you are using will have defined a set of additional MISFIRE_INSTRUCTION_XXX constants that may be passed to this method. ")]
        public SimpleTriggerMisfireInstruction MisfireInstruction {
            get {
                return _misfireInstruction;
            }
            set {
                SetPropertyValue("MisfireInstruction", ref _misfireInstruction, value);
            }
        }
        private int _repeatCount;
        [Tooltip("Get or set thhe number of times the SimpleTrigger should repeat, after which it will be automatically deleted. ")]
        public int RepeatCount {
            get {
                return _repeatCount;
            }
            set {
                SetPropertyValue("RepeatCount", ref _repeatCount, value);
            }
        }
        private TimeSpan _repeatInterval;
        [Tooltip("Get or set the the time interval at which the SimpleTrigger should repeat. ")]
        public TimeSpan RepeatInterval {
            get {
                return _repeatInterval;
            }
            set {
                SetPropertyValue("RepeatInterval", ref _repeatInterval, value);
            }
        }
        private int _timesTriggered;
        [Tooltip("Get or set the number of times the SimpleTrigger has already fired. ")]
        public int TimesTriggered {
            get {
                return _timesTriggered;
            }
            set {
                SetPropertyValue("TimesTriggered", ref _timesTriggered, value);
            }
        }
        private DateTime? _finalFireTimeUtc;
        [Tooltip("Returns the final UTC time at which the SimpleTrigger will fire, if repeatCount is RepeatIndefinitely, null will be returned. Note that the return time may be in the past. ")]
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