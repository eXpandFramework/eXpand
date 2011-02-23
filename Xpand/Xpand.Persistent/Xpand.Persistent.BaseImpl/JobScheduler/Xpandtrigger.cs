using System;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    public enum NthIncludedDayTriggerMisfireInstruction {
        InstructionNotSet,
        SmartPolicy,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the NthIncludedDayTrigger wants to be fired now by the IScheduler")]
        NthIncludedDayTriggerFireOnceNow,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the NthIncludedDayTrigger wants to have nextFireTime updated to the next time in the schedule after the current time, but it does not want to be fired now. ")]
        NthIncludedDayDoNothing,
    }
    public enum CronTriggerMisfireInstruction {
        InstructionNotSet,
        SmartPolicy,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the CronTrigger wants to be fired now by IScheduler. ")]
        CronTriggerFireOnceNow,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the CronTrigger wants to have it's next-fire-time updated to the next time in the schedule after the current time (taking into account any associated ICalendar, but it does not want to be fired now.")]
        CronTriggerDoNothing,
    }
    public abstract class XpandTrigger : XpandCustomObject, IXpandTrigger {
        protected XpandTrigger(Session session)
            : base(session) {
        }
        private string _name;
        public string Name {
            get {
                return _name;
            }
            set {
                SetPropertyValue("Name", ref _name, value);
            }
        }
        private string _group;
        public string Group {
            get {
                return _group;
            }
            set {
                SetPropertyValue("Group", ref _group, value);
            }
        }
        
        private string _jobName;
        [Tooltip("Get or set the name of the associated JobDetail. ")]
        public string JobName {
            get {
                return _jobName;
            }
            set {
                SetPropertyValue("JobName", ref _jobName, value);
            }
        }
        private string _jobGroup;
        [Tooltip("Gets or sets the name of the associated JobDetail's group. If set with null, Scheduler.DefaultGroup will be used. ")]
        public string JobGroup {
            get {
                return _jobGroup;
            }
            set {
                SetPropertyValue("JobGroup", ref _jobGroup, value);
            }
        }
        private string _description;
        [Size(SizeAttribute.Unlimited)]
        public string Description {
            get {
                return _description;
            }
            set {
                SetPropertyValue("Description", ref _description, value);
            }
        }
        private string _calendarName;
        [Tooltip("Get or set the ICalendar with the given name with this Trigger. Use null when setting to dis-associate a Calendar. ")]
        public string CalendarName {
            get {
                return _calendarName;
            }
            set {
                SetPropertyValue("CalendarName", ref _calendarName, value);
            }
        }
        private XpandJobDataMap _jobDataMap;
        public XpandJobDataMap JobDataMap {
            get {
                return _jobDataMap;
            }
            set {
                SetPropertyValue("JobDataMap", ref _jobDataMap, value);
            }
        }
        private bool _volatile;
        [Tooltip("Whether or not the Trigger should be persisted in the IJobStore for re-use after program restarts. ")]
        public bool Volatile {
            get {
                return _volatile;
            }
            set {
                SetPropertyValue("Volatile", ref _volatile, value);
            }
        }
        

        private DateTime? _endTimeUtc;
        [Tooltip("Returns the date/time on which the trigger must stop firing. This defines the final boundary for trigger firings 舒 the trigger will not fire after to this date and time. If this value is null, no end time boundary is assumed, and the trigger can continue indefinitely. Sets the date/time on which the trigger must stop firing. This defines the final boundary for trigger firings 舒 the trigger will not fire after to this date and time. If this value is null, no end time boundary is assumed, and the trigger can continue indefinitely. ")]
        public DateTime? EndTimeUtc {
            get {
                return _endTimeUtc;
            }
            set {
                SetPropertyValue("EndTimeUtc", ref _endTimeUtc, value);
            }
        }
        private DateTime _startTimeUtc;
        [Tooltip(@"The time at which the trigger's scheduling should start. May or may not be the first actual fire time of the trigger, depending upon the type of trigger and the settings of the other properties of the trigger. However the first actual first time will not be before this date. 
Remarks:
Setting a value in the past may cause a new trigger to compute a first fire time that is in the past, which may cause an immediate misfire of the trigger.")]
        [RuleRequiredField]
        public DateTime StartTimeUtc {
            get {
                return _startTimeUtc;
            }
            set {
                SetPropertyValue("StartTimeUtc", ref _startTimeUtc, value);
            }
        }
        private TriggerPriority _priority;
        public TriggerPriority Priority {
            get {
                return _priority;
            }
            set {
                SetPropertyValue("Priority", ref _priority, value);
            }
        }
    }

}