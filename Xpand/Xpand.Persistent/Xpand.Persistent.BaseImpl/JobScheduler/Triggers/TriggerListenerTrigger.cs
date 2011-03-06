
using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Triggers {
    public class TriggerListenerTrigger : XpandCustomObject, ITriggerListenerTrigger {
        public TriggerListenerTrigger(Session session)
            : base(session) {
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
        private TriggerListenerEvent _event;
        public TriggerListenerEvent Event {
            get {
                return _event;
            }
            set {
                SetPropertyValue("Event", ref _event, value);
            }
        }
        [ManyToManyAlias("Links", "JobTrigger")]
        public IList<XpandJobTrigger> JobTriggers {
            get { return GetList<XpandJobTrigger>("JobTriggers"); }
        }

        IList<IJobTrigger> ITriggerListenerTrigger.JobTriggers {
            get { return new ListConverter<IJobTrigger, XpandJobTrigger>(JobTriggers); }
        }

        [Association("TriggerListenerTrigger-JobTriggerTriggerListenerTriggerLinks"), AggregatedAttribute]
        protected IList<JobTriggerTriggerListenerTriggerLink> Links {
            get {
                return GetList<JobTriggerTriggerListenerTriggerLink>("Links");
            }
        }
    }

}
