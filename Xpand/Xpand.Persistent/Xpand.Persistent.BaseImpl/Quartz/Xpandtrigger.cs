using System;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.Quartz {
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
        private object _job;
        [Association("object-XpandTriggers")]
        public object Job {
            get {
                return _job;
            }
            set {
                SetPropertyValue("Job", ref _job, value);
            }
        }
        private string _jobGroup;
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
        public bool Volatile {
            get {
                return _volatile;
            }
            set {
                SetPropertyValue("Volatile", ref _volatile, value);
            }
        }
        private int _misfireInstruction;
        public int MisfireInstruction {
            get {
                return _misfireInstruction;
            }
            set {
                SetPropertyValue("MisfireInstruction", ref _misfireInstruction, value);
            }
        }
        private string _fireInstanceId;
        public string FireInstanceId {
            get {
                return _fireInstanceId;
            }
            set {
                SetPropertyValue("FireInstanceId", ref _fireInstanceId, value);
            }
        }
        private DateTime? _endTimeUtc;
        public DateTime? EndTimeUtc {
            get {
                return _endTimeUtc;
            }
            set {
                SetPropertyValue("EndTimeUtc", ref _endTimeUtc, value);
            }
        }
        private DateTime _startTimeUtc;
        [RuleRequiredField]
        public DateTime StartTimeUtc {
            get {
                return _startTimeUtc;
            }
            set {
                SetPropertyValue("StartTimeUtc", ref _startTimeUtc, value);
            }
        }
        private int _priority;
        public int Priority {
            get {
                return _priority;
            }
            set {
                SetPropertyValue("Priority", ref _priority, value);
            }
        }

    }
}