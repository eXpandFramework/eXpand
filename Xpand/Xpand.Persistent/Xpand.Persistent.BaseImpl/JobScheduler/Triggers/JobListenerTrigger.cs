using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Triggers {

    public class JobListenerTrigger : XpandCustomObject, IJobListenerTrigger {
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

        [Association("JobListenerTrigger-JobDetailJobListenerTriggerLinks"), AggregatedAttribute]
        protected IList<JobDetailJobListenerTriggerLink> Links {
            get {
                return GetList<JobDetailJobListenerTriggerLink>("Links");
            }
        }
        [ManyToManyAlias("Links", "JobDetail")]
        public IList<XpandJobDetail> JobDetails {
            get { return GetList<XpandJobDetail>("JobDetails"); }
        }

        IList<IJobDetail> IJobDetails.JobDetails {
            get { return new ListConverter<IJobDetail, XpandJobDetail>(JobDetails); }
        }
    }
}