using System;
using System.Globalization;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using TimeZoneId = Xpand.Persistent.Base.JobScheduler.Triggers.TimeZoneId;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Triggers {
    [NavigationItem("JobScheduler")]
    [System.ComponentModel.DisplayName("NthIncludedDayTrigger")]
    public class XpandNthIncludedDayTrigger : XpandJobTrigger, INthIncludedDayTrigger {
        public XpandNthIncludedDayTrigger(Session session)
            : base(session) {
        }
        private int _n;
        [Tooltip("Gets or sets the day of the interval on which the NthIncludedDayTrigger should fire. If the Nth day of the interval does not exist (i.e. the 32nd of a month), the trigger simply will never fire. N may not be less than 1. ")]
        public int N {
            get {
                return _n;
            }
            set {
                SetPropertyValue("N", ref _n, value);
            }
        }
        private NthIncludedDayIntervalType _intervalType;
        [Tooltip("@Returns the interval type for the NthIncludedDayTrigger. Remarks:Sets the interval type for the NthIncludedDayTrigger. If IntervalTypeMonthly, the trigger will fire on the Nth included day of every month. If IntervalTypeYearly, the trigger will fire on the Nth included day of every year. If IntervalTypeWeekly, the trigger will fire on the Nth included day of every week. ")]
        public NthIncludedDayIntervalType IntervalType {
            get {
                return _intervalType;
            }
            set {
                SetPropertyValue("IntervalType", ref _intervalType, value);
            }
        }
        private TimeSpan _fireAtTime;
        public TimeSpan FireAtTime {
            get {
                return _fireAtTime;
            }
            set {
                SetPropertyValue("FireAtTime", ref _fireAtTime, value);
            }
        }
        private int _nextFireCutoffInterval;
        [Tooltip(@"Because of the conceptual design of NthIncludedDayTrigger, it is not always possible to decide with certainty that the trigger will never fire again. Therefore, it will search for the next fire time up to a given cutoff. These cutoffs can be changed by using the NextFireCutoffInterval property. The default cutoff is 12 of the intervals specified by IntervalType intervalType. 
Because of the conceptual design of NthIncludedDayTrigger, it is not always possible to decide with certainty that the trigger will never fire again. Therefore, it will search for the next fire time up to a given cutoff. These cutoffs can be changed by using the NextFireCutoffInterval method. The default cutoff is 12 of the intervals specified by IntervalType intervalType
In most cases, the default value of this setting (12) is sufficient (it is highly unlikely, for example, that you will need to look at more than 12 months of dates to ensure that your trigger will never fire again). However, this setting is included to allow for the rare exceptions where this might not be true. 
For example, if your trigger is associated with a calendar that excludes a great many dates in the next 12 months, and hardly any following that, it is possible (if N is large enough) that you could run into this situation. 
")]
        public int NextFireCutoffInterval {
            get {
                return _nextFireCutoffInterval;
            }
            set {
                SetPropertyValue("NextFireCutoffInterval", ref _nextFireCutoffInterval, value);
            }
        }
        private TimeZoneId _timeZone;
        public TimeZoneId TimeZone {
            get {
                return _timeZone;
            }
            set {
                SetPropertyValue("TimeZone", ref _timeZone, value);
            }
        }
        private DayOfWeek _triggerCalendarFirstDayOfWeek;
        public DayOfWeek TriggerCalendarFirstDayOfWeek {
            get {
                return _triggerCalendarFirstDayOfWeek;
            }
            set {
                SetPropertyValue("TriggerCalendarFirstDayOfWeek", ref _triggerCalendarFirstDayOfWeek, value);
            }
        }
        private CalendarWeekRule _triggerCalendarWeekRule;
        public CalendarWeekRule TriggerCalendarWeekRule {
            get {
                return _triggerCalendarWeekRule;
            }
            set {
                SetPropertyValue("TriggerCalendarWeekRule", ref _triggerCalendarWeekRule, value);
            }
        }
        private NthIncludedDayTriggerMisfireInstruction _misfireInstruction;
        public NthIncludedDayTriggerMisfireInstruction MisfireInstruction {
            get {
                return _misfireInstruction;
            }
            set {
                SetPropertyValue("MisfireInstruction", ref _misfireInstruction, value);
            }
        }
    }
}