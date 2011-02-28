using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Quartz.Util;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobTriggerLinkController : SupportSchedulerController {
        public JobTriggerLinkController() {
            TargetObjectType = typeof(IJobDetail);
        }

        public event EventHandler<CustomNonDeletedObjectsToSaveArgs> CustomNonDeletedObjectsToSave;

        protected void OnCustomNonDeletedObjectsToSave(CustomNonDeletedObjectsToSaveArgs e) {
            EventHandler<CustomNonDeletedObjectsToSaveArgs> handler = CustomNonDeletedObjectsToSave;
            if (handler != null) handler(this, e);
        }


        readonly List<JobDetailTriggerInfo> supportJobDetails = new List<JobDetailTriggerInfo>();

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleting -= ObjectSpaceOnObjectDeleting;
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleting += ObjectSpaceOnObjectDeleting;
        }

        void ObjectSpaceOnObjectDeleting(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {
            supportJobDetails.AddRange(objectsManipulatingEventArgs.Objects.OfType<ISupportSchedulerLink>().SelectMany(CreateJobDetailInfos()));
        }

        Func<ISupportSchedulerLink, IEnumerable<JobDetailTriggerInfo>> CreateJobDetailInfos() {
            return link => {
                if (link is ISupportJobDetail)
                    return CreateJobDetailInfosCore(((ISupportJobDetail)link));
                return CreateJobDetailInfosCore((IJobSchedulerGroupTriggerLink)link);
            };
        }

        IEnumerable<JobDetailTriggerInfo> CreateJobDetailInfosCore(IJobSchedulerGroupTriggerLink jobSchedulerGroupTriggerLink) {
            var jobDetails = ObjectSpace.GetObjects(View.ObjectTypeInfo.Type, ForTheSameGroup(jobSchedulerGroupTriggerLink)).OfType<IJobDetail>();
            return jobDetails.Select(detail => new JobGroupInfo(detail.Name, detail.Job.JobType, detail.Group.Name, jobSchedulerGroupTriggerLink.Trigger.Name)).OfType<JobDetailTriggerInfo>();
        }

        IEnumerable<JobDetailTriggerInfo> CreateJobDetailInfosCore(ISupportJobDetail supportJobDetail) {
            var jobType = supportJobDetail.JobDetail.Job.JobType;
            if (supportJobDetail is IJobDetailJobListenerTriggerLink) {
                var jobListenerTrigger = ((IJobDetailJobListenerTriggerLink)supportJobDetail).JobListenerTrigger;
                jobType = jobListenerTrigger.JobType;
                return new List<JobDetailTriggerInfo> { new JobListenerInfo(supportJobDetail.JobDetail.Name, jobType, jobListenerTrigger.Event, supportJobDetail.JobDetail.Job.JobType) };
            }
            return new List<JobDetailTriggerInfo> { new JobDetailTriggerInfo(supportJobDetail.JobDetail.Name, jobType, ((IJobDetailTriggerLink)supportJobDetail).JobTrigger.Name) };
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            Save();
            Delete();
        }

        void AddJobListeners(IJobDetailJobListenerTriggerLink link) {
            List<IJobDetail> jobDetails = GetRelatedJobDetails(ForTheSameJobType(link.JobListenerTrigger.JobType));
            jobDetails.ForEach(detail => GetListenerDataMap(detail, AddJobListener, link.JobDetail.Name, link.JobDetail.Job.JobType, link.JobListenerTrigger.Event));
        }

        Func<CriteriaOperator> ForTheSameJobType(Type type) {
            return () => CriteriaOperator.Parse("Job.JobType=?", type);
        }

        List<IJobDetail> GetRelatedJobDetails(Func<CriteriaOperator> action) {
            var type = TypesInfo.FindBussinessObjectType<IJobDetail>();
            return ObjectSpace.GetObjects(type, action.Invoke()).OfType<IJobDetail>().ToList();
        }

        void Delete() {
            supportJobDetails.OfType<JobListenerInfo>().ToList().ForEach(RemoveJobListeners);
            supportJobDetails.OfType<JobDetailTriggerInfo>().ToList().ForEach(UnscheduleJob);
            supportJobDetails.Clear();
        }

        void UnscheduleJob(JobDetailTriggerInfo jobTriggerInfo) {
            var jobGroup = jobTriggerInfo is JobGroupInfo?((JobGroupInfo)jobTriggerInfo).Group:null;
            Scheduler.UnscheduleJob(jobTriggerInfo.TriggerName, jobTriggerInfo.JobType, jobTriggerInfo.JobName,jobGroup);
        }

        void Save() {
            var customNonDeletedObjectsToSaveArgs = new CustomNonDeletedObjectsToSaveArgs(ObjectSpace.GetNonDeletedObjectsToSave<object>());
            OnCustomNonDeletedObjectsToSave(customNonDeletedObjectsToSaveArgs);
            customNonDeletedObjectsToSaveArgs.Objects.OfType<IJobDetailJobListenerTriggerLink>().ToList().ForEach(AddJobListeners);
            customNonDeletedObjectsToSaveArgs.Objects.OfType<IJobDetailTriggerLink>().ToList().ForEach(ScheduleJob);
            customNonDeletedObjectsToSaveArgs.Objects.OfType<IJobSchedulerGroupTriggerLink>().ToList().ForEach(ScheduleGroup);
        }

        void ScheduleJob(IJobDetailTriggerLink link) {
            var simpleTrigger = Mapper.GetSimpleTrigger(link.JobTrigger, link.JobDetail.Name, link.JobDetail.Job.JobType, null);
            Scheduler.ScheduleJob(simpleTrigger);
        }

        void ScheduleGroup(IJobSchedulerGroupTriggerLink link) {
            var relatedJobDetails = GetRelatedJobDetails(() => ForTheSameGroup(link));
            relatedJobDetails.ForEach(detail => {
                var simpleTrigger = Mapper.GetSimpleTrigger(link.Trigger, detail.Name, detail.Job.JobType, link.JobSchedulerGroup.Name);
                Scheduler.ScheduleJob(simpleTrigger);
            });
        }

        CriteriaOperator ForTheSameGroup(IJobSchedulerGroupTriggerLink link) {
            return CriteriaOperator.Parse("Group.Name=?", link.JobSchedulerGroup.Name);
        }

        void RemoveJobListeners(JobListenerInfo jobListenerInfo) {
            List<IJobDetail> jobDetails = GetRelatedJobDetails(ForTheSameJobType(jobListenerInfo.JobType));
            jobDetails.ForEach(detail => GetListenerDataMap(detail, RemoveJobListener, jobListenerInfo.JobName, jobListenerInfo.OriginType, jobListenerInfo.ListenerEvent));
        }

        void RemoveJobListener(List<Key> list, Key key) {
            list.Remove(key);
        }

        void AddJobListener(List<Key> list, Key key) {
            list.Add(key);
        }

        void GetListenerDataMap(IJobDetail detail, Action<List<Key>, Key> action, string jobName, Type jobType, JobListenerEvent listenerEvent) {
            var jobDetail = Scheduler.GetJobDetail(detail);
            var jobDataMapKeyCalculator = new JobDataMapKeyCalculator();
            var listenerNames = jobDataMapKeyCalculator.GetListenerNames(jobDetail.JobDataMap, listenerEvent);
            action.Invoke(listenerNames, new Key(jobName, jobType.FullName));
            jobDataMapKeyCalculator.CreateListenersKey(jobDetail.JobDataMap, listenerEvent, listenerNames.ToArray());
            Scheduler.StoreJob(jobDetail);
        }
    }

    public class CustomNonDeletedObjectsToSaveArgs : HandledEventArgs {
        public CustomNonDeletedObjectsToSaveArgs(IEnumerable<object> objects) {
            Objects = objects;
        }

        public IEnumerable<object> Objects { get; set; }
    }


    internal class JobListenerInfo : JobDetailTriggerInfo {
        readonly JobListenerEvent _listenerEvent;
        readonly Type _originType;

        public JobListenerInfo(string jobName, Type jobType, JobListenerEvent listenerEvent, Type originType)
            : base(jobName, jobType,null) {
            _listenerEvent = listenerEvent;
            _originType = originType;
        }

        public JobListenerEvent ListenerEvent {
            get { return _listenerEvent; }
        }

        public Type OriginType {
            get { return _originType; }
        }
    }

    internal class JobGroupInfo : JobDetailTriggerInfo, IJobTriggerInfo {
        readonly string _group;

        public JobGroupInfo(string jobName, Type jobType, string group, string triggerName)
            : base(jobName, jobType,triggerName) {
            _group = group;
        }

        public string Group {
            get { return _group; }
        }

    }

    internal interface IJobTriggerInfo {
        string TriggerName { get; }
    }

    internal  class JobDetailTriggerInfo {
        readonly Type _jobType;
        readonly string _triggerName;
        //        readonly LinkType _linkType;
        readonly string _jobName;

        public JobDetailTriggerInfo(string jobName, Type jobType, string triggerName) {
            _jobName = jobName;
            _jobType = jobType;
            _triggerName = triggerName;

            //            _jobListenerTriggerLink = supportSchedulerLink is IJobDetailJobListenerTriggerLink;
            //            if (!_jobListenerTriggerLink) {
            //                _triggerName = ((IJobDetailTriggerLink)supportSchedulerLink).JobTrigger.Name;
            //            }
            //            JobName = supportSchedulerLink.JobDetail.Name;
            //
            //            if (_jobListenerTriggerLink) {
            //                var jobListenerTrigger = ((IJobDetailJobListenerTriggerLink)supportSchedulerLink).JobListenerTrigger;
            //                JobType = jobListenerTrigger.JobType;
            //                _listenerEvent = jobListenerTrigger.Event;
            //            } else JobType = supportSchedulerLink.JobDetail.Job.JobType;
            //            OriginType = supportSchedulerLink.JobDetail.Job.JobType;
            //            _linkType = linkType;
        }



        //        public string TriggerName {
        //            get { return _triggerName; }
        //        }

        //        public LinkType LinkType {
        //            get { return _linkType; }
        //        }

        public string TriggerName {
            get { return _triggerName; }
        }

        public string JobName {
            get { return _jobName; }
        }

        public Type JobType {
            get { return _jobType; }

        }

        //        public Type OriginType { get; set; }
    }
}