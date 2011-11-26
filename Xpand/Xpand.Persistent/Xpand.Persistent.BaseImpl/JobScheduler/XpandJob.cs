using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    public class XpandJob : XpandCustomObject, IXpandJob {
        public XpandJob(Session session)
            : base(session) {
        }
        private string _name;
        [RuleRequiredField]
        public string Name {
            get {
                return _name;
            }
            set {
                SetPropertyValue("Name", ref _name, value);
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

        IDataMap IXpandJob.DataMap {
            get { return JobDataMap; }
            set { JobDataMap = value as XpandJobDataMap; }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if (JobType != null && string.IsNullOrEmpty(Name))
                Name = JobType.Name;
        }

        [Browsable(false)]
        public XPCollection<XpandJobDataMap> JobDataMaps {
            get {
                return JobType == null
                           ? new XPCollection<XpandJobDataMap>(Session, false)
                           : new XPCollection<XpandJobDataMap>(Session, DataMapTypeAttribute.GetCriteria<JobDataMapTypeAttribute>(Session, JobType));
            }
        }
        [Browsable(false)]
        public List<Type> JobDataMapTypes {
            get {
                return JobType == null ? new List<Type>() : XafTypesInfo.Instance.FindTypeInfo(JobType).FindAttributes<JobDataMapTypeAttribute>().Select(attribute => attribute.Type).ToList();
            }
        }

        private XpandJobDataMap _jobDataMap;
        [DataSourceProperty("JobDataMaps")]
        [NewObjectCollectCreatableItemTypesDataSource("JobDataMapTypes")]
        public XpandJobDataMap JobDataMap {
            get {
                return _jobDataMap;
            }
            set {
                SetPropertyValue("JobDataMap", ref _jobDataMap, value);
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
    }
}
