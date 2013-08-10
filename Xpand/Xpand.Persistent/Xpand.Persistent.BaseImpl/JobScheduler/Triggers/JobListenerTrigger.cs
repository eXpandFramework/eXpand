using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.ValueConverters;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Triggers {
    [Appearance("Disable_JobType_For_JobListenerTrigger_ExistingObjects", AppearanceItemType.ViewItem, "IsNewObject=false", TargetItems = "JobType", Enabled = false)]
    [Appearance("Disable_Group_For_JobListenerTrigger_ExistingObjects", AppearanceItemType.ViewItem, "IsNewObject=false", TargetItems = "Group", Enabled = false)]
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
        [RuleRequiredField(TargetCriteria = "Group Is Null")]
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
        private JobSchedulerGroup _group;
        [RuleRequiredField(TargetCriteria = "JobType Is Null")]
        [ProvidedAssociation("JobSchedulerGroup-JobListenerTriggers")]
        public JobSchedulerGroup Group {
            get {
                return _group;
            }
            set {
                SetPropertyValue("Group", ref _group, value);
            }
        }
        IJobSchedulerGroup IJobListenerTrigger.Group {
            get { return Group; }
            set { Group=value as JobSchedulerGroup; }
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

        IList<IXpandJobDetail> IJobDetails.JobDetails {
            get { return new ListConverter<IXpandJobDetail, XpandJobDetail>(JobDetails); }
        }
    }
}