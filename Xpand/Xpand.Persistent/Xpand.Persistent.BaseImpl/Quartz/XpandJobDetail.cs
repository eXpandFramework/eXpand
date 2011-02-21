using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.Core;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.Quartz {
    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("JobDetail")]
    public class XpandJobDetail : XpandCustomObject {
        public XpandJobDetail(Session session)
            : base(session) {
        }
        private string _name;
        [RuleRequiredField]
        [RuleUniqueValue(null, DefaultContexts.Save)]
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
        private Type _jobType;
        [RuleRequiredField]
        [ValueConverter(typeof(TypeValueConverter))]
        public Type JobType {
            get {
                return _jobType;
            }
            set {
                SetPropertyValue("JobType", ref _jobType, value);
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
        private bool _requestsRecovery;
        [Tooltip("Whether or not the the IScheduler should re-Execute the IJob if a 'recovery' or 'fail-over' situation is encountered.")]
        public bool RequestsRecovery {
            get {
                return _requestsRecovery;
            }
            set {
                SetPropertyValue("RequestsRecovery", ref _requestsRecovery, value);
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
        private bool _durable;
        public bool Durable {
            get {
                return _durable;
            }
            set {
                SetPropertyValue("Durable", ref _durable, value);
            }
        }

    }
}
