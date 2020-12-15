
using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Triggers {
    [Appearance("Disable_JobType_For_TriggerListenerTrigger_ExistingObjects", AppearanceItemType.ViewItem, "IsNewObject=false", TargetItems = "JobType", Enabled = false)]
    [Appearance("Disable_Group_For_TriggerListenerTrigger_ExistingObjects", AppearanceItemType.ViewItem, "IsNewObject=false", TargetItems = "Group", Enabled = false)]
    public class TriggerListenerTrigger : XpandCustomObject, ITriggerListenerTrigger, IFastManyToMany {
        public TriggerListenerTrigger(Session session)
            : base(session) {
        }
        private Type _jobType;
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(JobTypeClassInfoConverter))]
        [RuleRequiredField(TargetCriteria = "Group Is Null")]
        public Type JobType {
            get => _jobType;
            set => SetPropertyValue("JobType", ref _jobType, value);
        }
        private JobSchedulerGroup _group;
        [RuleRequiredField(TargetCriteria = "JobType Is Null")]
        [ProvidedAssociation("JobSchedulerGroup-TriggerListenerTriggers")]
        public JobSchedulerGroup Group {
            get => _group;
            set => SetPropertyValue("Group", ref _group, value);
        }
        IJobSchedulerGroup ITriggerListenerTrigger.Group {
            get => Group;
            set => Group = value as JobSchedulerGroup;
        }

        private TriggerListenerEvent _event;
        public TriggerListenerEvent Event {
            get => _event;
            set => SetPropertyValue("Event", ref _event, value);
        }
        [ManyToManyAlias("Links", "JobTrigger")]
        public IList<XpandJobTrigger> JobTriggers => GetList<XpandJobTrigger>();

        IList<IXpandJobTrigger> ITriggerListenerTrigger.JobTriggers => new ListConverter<IXpandJobTrigger, XpandJobTrigger>(JobTriggers);

        [Association("TriggerListenerTrigger-JobTriggerTriggerListenerTriggerLinks"), AggregatedAttribute]
        protected IList<JobTriggerTriggerListenerTriggerLink> Links => GetList<JobTriggerTriggerListenerTriggerLink>();
    }

}
