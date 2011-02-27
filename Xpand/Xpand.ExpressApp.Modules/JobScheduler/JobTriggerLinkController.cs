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
        readonly List<JobDetailInfo> supportJobDetails = new List<JobDetailInfo>();
        public JobTriggerLinkController() {
            TargetObjectType = typeof(IJobDetail);
        }

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
            supportJobDetails.AddRange(objectsManipulatingEventArgs.Objects.OfType<ISupportJobDetail>().Select(detail => new JobDetailInfo(detail)));
        }



        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            Save();
            Delete();
        }

        void AddJobListeners(IJobDetailJobListenerTriggerLink link) {
            List<IJobDetail> jobDetails = GetRelatedJobDetails(link.JobListenerTrigger.JobType);
            jobDetails.ForEach(detail => GetListenerDataMap(detail, AddJobListener, link.JobDetail.Name, link.JobDetail.Job.JobType, link.JobListenerTrigger.Event));
        }

        List<IJobDetail> GetRelatedJobDetails(Type jobType) {
            var type = TypesInfo.FindBussinessObjectType<IJobDetail>();
            var criteriaOperator = CriteriaOperator.Parse("Job.JobType=?", jobType);
            return ObjectSpace.GetObjects(type, criteriaOperator).OfType<IJobDetail>().ToList();
        }

        void Delete() {
            supportJobDetails.Where(info => info.JobListenerTriggerLink).ToList().ForEach(RemoveJobListeners);
            supportJobDetails.Where(info => !info.JobListenerTriggerLink).ToList().ForEach(UnscheduleJob);
            supportJobDetails.Clear();
        }

        void UnscheduleJob(JobDetailInfo jobDetailInfo) {
            Scheduler.UnscheduleJob(jobDetailInfo.TriggerName, jobDetailInfo.JobType,jobDetailInfo.JobName);
        }

        void Save() {
            ObjectSpace.GetNonDeletedObjectsToSave<IJobDetailJobListenerTriggerLink>().ToList().ForEach(AddJobListeners);
            ObjectSpace.GetNonDeletedObjectsToSave<IJobDetailTriggerLink>().ToList().ForEach(ScheduleJob);
        }

        void RemoveJobListeners(JobDetailInfo detailInfo) {
            List<IJobDetail> jobDetails = GetRelatedJobDetails(detailInfo.JobType);
            jobDetails.ForEach(detail => GetListenerDataMap(detail, RemoveJobListener, detailInfo.JobName, detailInfo.OriginType, detailInfo.ListenerEvent));
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
            var listenerNames = jobDataMapKeyCalculator.GetListenerNames(jobDetail.JobDataMap,listenerEvent);
            action.Invoke(listenerNames, new Key(jobName, jobType.FullName));
            jobDataMapKeyCalculator.CreateListenersKey(jobDetail.JobDataMap,listenerEvent, listenerNames.ToArray());
            Scheduler.StoreJob(jobDetail);
        }

        void ScheduleJob(IJobDetailTriggerLink link) {
            var simpleTrigger = Mapper.GetSimpleTrigger(link.JobTrigger, link.JobDetail.Name, link.JobDetail.Job.JobType);
            Scheduler.ScheduleJob(simpleTrigger);
        }
    }

    internal class JobDetailInfo {
        readonly bool _jobListenerTriggerLink;
        readonly string _triggerName;
        readonly JobListenerEvent _listenerEvent;

        public JobDetailInfo(ISupportJobDetail supportJobDetail) {
            _jobListenerTriggerLink = supportJobDetail is IJobDetailJobListenerTriggerLink;
            if (!_jobListenerTriggerLink) {
                _triggerName = ((IJobDetailTriggerLink)supportJobDetail).JobTrigger.Name;
            }
            JobName = supportJobDetail.JobDetail.Name;

            if (_jobListenerTriggerLink) {
                var jobListenerTrigger = ((IJobDetailJobListenerTriggerLink) supportJobDetail).JobListenerTrigger;
                JobType = jobListenerTrigger.JobType;
                _listenerEvent = jobListenerTrigger.Event;
            }
            else JobType = supportJobDetail.JobDetail.Job.JobType;
            OriginType = supportJobDetail.JobDetail.Job.JobType;
        }

        public JobListenerEvent ListenerEvent {
            get { return _listenerEvent; }
        }

        public string TriggerName {
            get { return _triggerName; }
        }

        public bool JobListenerTriggerLink {
            get { return _jobListenerTriggerLink; }
        }

        public string JobName { get; set; }

        public Type JobType { get; set; }
        public Type OriginType { get; set; }
    }
}