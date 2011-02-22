using System;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.Quartz {
    public enum SimpleTriggerMisfireInstruction {
        InstructionNotSet,
        SmartPolicy,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the SimpleTrigger wants to be fired now by IScheduler. 
NOTE: This instruction should typically only be used for 'one-shot' (non-repeating) Triggers. If it is used on a trigger with a repeat count > 0 then it is equivalent to the instruction RescheduleNowWithRemainingRepeatCount . 
")]
        FireNow,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the SimpleTrigger wants to be re-scheduled to 'now' (even if the associated ICalendar excludes 'now') with the repeat count left as-is. This does obey the Trigger end-time however, so if 'now' is after the end-time the Trigger will not fire again. 
Remarks:
NOTE: Use of this instruction causes the trigger to 'forget' the start-time and repeat-count that it was originally setup with (this is only an issue if you for some reason wanted to be able to tell what the original values were at some later time). 
")]
        RescheduleNowWithExistingRepeatCount,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the SimpleTrigger wants to be re-scheduled to 'now' (even if the associated ICalendar excludes 'now') with the repeat count set to what it would be, if it had not missed any firings. This does obey the Trigger end-time however, so if 'now' is after the end-time the Trigger will not fire again. 
NOTE: Use of this instruction causes the trigger to 'forget' the start-time and repeat-count that it was originally setup with. Instead, the repeat count on the trigger will be changed to whatever the remaining repeat count is (this is only an issue if you for some reason wanted to be able to tell what the original values were at some later time). 

NOTE: This instruction could cause the Trigger to go to the 'COMPLETE' state after firing 'now', if all the repeat-fire-times where missed. 
")]
        RescheduleNowWithRemainingRepeatCount,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the SimpleTrigger wants to be re-scheduled to the next scheduled time after 'now' - taking into account any associated ICalendar, and with the repeat count set to what it would be, if it had not missed any firings. 
Remarks:
NOTE/WARNING: This instruction could cause the Trigger to go directly to the 'COMPLETE' state if all fire-times where missed. ")]
        RescheduleNextWithRemainingCount,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the SimpleTrigger wants to be re-scheduled to the next scheduled time after 'now' - taking into account any associated ICalendar, and with the repeat count left unchanged. 
Remarks:
NOTE/WARNING: This instruction could cause the Trigger to go directly to the 'COMPLETE' state if all the end-time of the trigger has arrived.
")]
        RescheduleNextWithExistingCount,

    }
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
    public abstract class XpandTrigger : XpandCustomObject {
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

    public enum TriggerPriority {
        Default=5
    }
}