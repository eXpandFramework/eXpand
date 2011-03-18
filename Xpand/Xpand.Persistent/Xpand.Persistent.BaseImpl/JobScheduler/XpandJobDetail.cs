using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Quartz;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Persistent.BaseImpl.JobScheduler.Triggers;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    [System.ComponentModel.DisplayName("JobDetail")]
    [Appearance("Disable_Name_For_XpandJobDetail_ExistingObjects", AppearanceItemType.ViewItem, "IsNewObject=false", TargetItems = "Name", Enabled = false)]
    [Appearance("Disable_Job_For_XpandJobDetail_ExistingObjects", AppearanceItemType.ViewItem, "IsNewObject=false", TargetItems = "Job", Enabled = false)]
    [Appearance("Disable_Group_For_XpandJobDetail_ExistingObjects", AppearanceItemType.ViewItem, "IsNewObject=false", TargetItems = "Group", Enabled = false)]
    [Appearance("Disable_JobDataMap_For_XpandJobDetail_When_Job_is_Null", AppearanceItemType.ViewItem, "Job Is Null", TargetItems = "JobDataMap", Enabled = false)]
    [NavigationItem("JobScheduler")]
    public class XpandJobDetail : XpandCustomObject, IXpandJobDetail, IFastManyToMany, IRequireSchedulerInitialization {
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
        [Tooltip("Whether or not the IJob implements the interface IStatefulJob.")]
        public virtual bool Stateful {
            get {
                return _job != null && (XafTypesInfo.Instance.FindTypeInfo(typeof(IStatefulJob)).Type.IsAssignableFrom(_job.JobType));
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
        private XpandJob _job;
        [ProvidedAssociation("XpandJob-XpandJobDetails")]
        [RuleRequiredField]
        [ImmediatePostData]
        public XpandJob Job {
            get {
                return _job;
            }
            set {
                SetPropertyValue("Job", ref _job, value);
            }
        }
        IXpandJob IXpandJobDetail.Job {
            get { return _job; }
            set { _job = value as XpandJob; }
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


        [Association("JobDetailTriggerLink-Triggers"), Aggregated]
        protected IList<JobDetailTriggerLink> TriggerLinks {
            get {
                return GetList<JobDetailTriggerLink>("TriggerLinks");
            }
        }
        [ManyToManyAlias("TriggerLinks", "JobTrigger")]
        public IList<XpandJobTrigger> JobTriggers {
            get { return GetList<XpandJobTrigger>("JobTriggers"); }
        }
        IList<IXpandJobTrigger> IXpandJobDetail.JobTriggers {
            get {
                return new ListConverter<IXpandJobTrigger, XpandJobTrigger>(JobTriggers);
            }
        }
        [Association("JobDetailJobListenerTriggerLink-JobListeners"), Aggregated]
        protected IList<JobDetailJobListenerTriggerLink> JobListenerTriggerLinks {
            get {
                return GetList<JobDetailJobListenerTriggerLink>("JobListenerTriggerLinks");
            }
        }
        [ManyToManyAlias("JobListenerTriggerLinks", "JobListenerTrigger")]
        public IList<JobListenerTrigger> JobListenerTriggers {
            get { return GetList<JobListenerTrigger>("JobListenerTriggers"); }
        }
        private JobSchedulerGroup _group;
        [ProvidedAssociation("JobSchedulerGroup-XpandJobDetails")]
        public JobSchedulerGroup Group {
            get {
                return _group;
            }
            set {
                SetPropertyValue("Group", ref _group, value);
            }
        }
        private XpandJobDetailDataMap _jobDetailDataMap;
        [RuleRequiredField]
        [DataSourceProperty("JobDetailDataMaps")]
        [NewObjectCollectCreatableItemTypesDataSourceAttribute("JobDetailDataMapTypes")]
        public XpandJobDetailDataMap JobDetailDataMap {
            get {
                return _jobDetailDataMap;
            }
            set {
                SetPropertyValue("JobDataMap", ref _jobDetailDataMap, value);
            }
        }

        [Browsable(false)]
        public XPCollection<XpandJobDetailDataMap> JobDetailDataMaps {
            get {
                return Job == null
                           ? new XPCollection<XpandJobDetailDataMap>(Session, false)
                           : new XPCollection<XpandJobDetailDataMap>(Session, DataMapTypeAttribute.GetCriteria<JobDetailDataMapTypeAttribute>(Session, Job.JobType));
            }
        }
        [Browsable(false)]
        public List<Type> JobDetailDataMapTypes {
            get {
                return Job == null ? new List<Type>() : XafTypesInfo.Instance.FindTypeInfo(Job.JobType).FindAttributes<JobDetailDataMapTypeAttribute>().Select(attribute => attribute.Type).ToList();
            }
        }

        IDataMap IXpandJobDetail.JobDataMap {
            get { return JobDetailDataMap; }
            set { JobDetailDataMap = value as XpandJobDetailDataMap; }
        }


        IJobSchedulerGroup IXpandJobDetail.Group {
            get { return Group; }
            set { Group = value as JobSchedulerGroup; }
        }

        IList<IJobListenerTrigger> IXpandJobDetail.JobListenerTriggers {
            get {
                return new ListConverter<IJobListenerTrigger, JobListenerTrigger>(JobListenerTriggers);
            }
        }
    }
}
