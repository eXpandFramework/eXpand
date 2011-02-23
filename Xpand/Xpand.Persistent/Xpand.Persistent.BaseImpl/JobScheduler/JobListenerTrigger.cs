using System;
using System.ComponentModel;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    public enum JobListenerEvent {
        Executing,
        Vetoed,
        Executed
    }

    public class JobListenerTrigger : XpandCustomObject, ISupportJobDetails {
        public JobListenerTrigger(Session session)
            : base(session) {
        }
        private JobListenerEvent _event;
        public JobListenerEvent Event {
            get {
                return _event;
            }
            set {
                SetPropertyValue("Event", ref _event, value);
            }
        }

        private Type _jobType;
        [RuleRequiredField]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(JobTypeClassInfoConverter))]
        public Type JobType {
            get {
                return _jobType;
            }
            set {
                SetPropertyValue("JobType", ref _jobType, value);
            }
        }
        [ProvidedAssociation("JobListenerTrigger-JobDetails", RelationType.ManyToMany)]
        public XPCollection<XpandJobDetail> JobDetails {
            get {
                return GetCollection<XpandJobDetail>("JobDetails");
            }
        }

    }
}