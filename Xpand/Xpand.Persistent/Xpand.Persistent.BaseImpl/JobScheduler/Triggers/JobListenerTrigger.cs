using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
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
            get => _event;
            set => SetPropertyValue("Event", ref _event, value);
        }

        private Type _jobType;
        [RuleRequiredField(TargetCriteria = "Group Is Null")]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(JobTypeClassInfoConverter))]
        public Type JobType {
            get => _jobType;
            set => SetPropertyValue("JobType", ref _jobType, value);
        }
        private JobSchedulerGroup _group;
        [RuleRequiredField(TargetCriteria = "JobType Is Null")]
        [ProvidedAssociation("JobSchedulerGroup-JobListenerTriggers")]
        public JobSchedulerGroup Group {
            get => _group;
            set => SetPropertyValue("Group", ref _group, value);
        }
        IJobSchedulerGroup IJobListenerTrigger.Group {
            get => Group;
            set => Group=value as JobSchedulerGroup;
        }

        [Association("JobListenerTrigger-JobDetailJobListenerTriggerLinks"), AggregatedAttribute]
        protected IList<JobDetailJobListenerTriggerLink> Links => GetList<JobDetailJobListenerTriggerLink>();

        [ManyToManyAlias("Links", "JobDetail")]
        public IList<XpandJobDetail> JobDetails => GetList<XpandJobDetail>();

        IList<IXpandJobDetail> IJobDetails.JobDetails => new ListConverter<IXpandJobDetail, XpandJobDetail>(JobDetails);
    }
}