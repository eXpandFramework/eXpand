using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Xpo.Converters.ValueConverters;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Triggers {
    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("SimpleTrigger")]
    public class XpandSimpleTrigger : XpandJobTrigger, ISimpleTrigger {
        public XpandSimpleTrigger(Session session)
            : base(session) {
        }
        private SimpleTriggerMisfireInstruction _misfireInstruction;
        void ISimpleTrigger.SetFinalFireTimeUtc(DateTime? dateTime) {
            _finalFireTimeUtc = dateTime;
            if (dateTime != null)
                StartTimeUtc = dateTime.Value;
        }

        [DisplayDateAndTime]
        public DateTime Now {
            get { return DateTime.UtcNow; }
        }

        [Tooltip("Get or set the instruction the IScheduler should be given for handling misfire situations for this Trigger- the concrete Trigger type that you are using will have defined a set of additional MISFIRE_INSTRUCTION_XXX constants that may be passed to this method. ")]
        public SimpleTriggerMisfireInstruction MisfireInstruction {
            get {
                return _misfireInstruction;
            }
            set {
                SetPropertyValue("MisfireInstruction", ref _misfireInstruction, value);
            }
        }
        private int? _repeatCount;
        [Tooltip("Get or set thhe number of times the SimpleTrigger should repeat, after which it will be automatically deleted. ")]
        [RuleRequiredField(TargetCriteria = "RepeatInterval is not null")]
        public int? RepeatCount {
            get {
                return _repeatCount;
            }
            set {
                SetPropertyValue("RepeatCount", ref _repeatCount, value);
            }
        }
        private TimeSpan? _repeatInterval;
        [RuleRequiredField(TargetCriteria = "RepeatCount is not null")]
        [Tooltip("Get or set the the time interval at which the SimpleTrigger should repeat. ")]
        public TimeSpan? RepeatInterval {
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
        [Persistent("FinalFireTimeUtc")]
        [ValueConverter(typeof(SqlDateTimeOverFlowValueConverter))]
        private DateTime? _finalFireTimeUtc;
        [Tooltip("Returns the final UTC time at which the SimpleTrigger will fire, if repeatCount is RepeatIndefinitely, null will be returned. Note that the return time may be in the past. ")]

        [DisplayDateAndTime]
        public DateTime? FinalFireTimeUtc {
            get {
                return _finalFireTimeUtc;
            }
        }
    }
}