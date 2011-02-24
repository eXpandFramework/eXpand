using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Quartz;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobSchedulerController : SupportSchedulerController {
        readonly List<JobDetailInfo> supportJobDetails = new List<JobDetailInfo>();
        public JobSchedulerController() {
            TargetObjectType = typeof(IJobDetail);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleting-=ObjectSpaceOnObjectDeleting;
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleting += ObjectSpaceOnObjectDeleting;
        }

        void ObjectSpaceOnObjectDeleting(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {
            supportJobDetails.AddRange(objectsManipulatingEventArgs.Objects.OfType<ISupportJobDetail>().Select(detail =>new JobDetailInfo(detail)));
        }


        void RemoveJobListeners(IJobDetail jobDetail, IJobListener jobListener) {
            var detail = Scheduler.GetJobDetail(jobDetail.Name,jobDetail.Group);
            detail.RemoveJobListener(jobListener.Name);
            Scheduler.RemoveJobListener(jobListener.Name);
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            Save();
            Delete();
        }

        void Delete() {
            supportJobDetails.Where(info => info.JobListenerTriggerLink).ToList().ForEach(Delete);
            supportJobDetails.Where(info => !info.JobListenerTriggerLink).ToList().ForEach(UnscheduleJob);
            supportJobDetails.Clear();
        }

        void UnscheduleJob(JobDetailInfo jobDetailInfo) {
            Scheduler.UnscheduleJob(jobDetailInfo.TriggerName, jobDetailInfo.JobGroup);
        }

        private void Delete(JobDetailInfo detailInfo) {
            CreateJobListenersInstances((listener, detail) => RemoveJobListeners(detail, listener), detailInfo.JobType);
        }
        void Save() {
            ObjectSpace.GetNonDeletedObjectsToSave<IJobDetailJobListenerTriggerLink>().ToList().ForEach(AddJobListeners);
            ObjectSpace.GetNonDeletedObjectsToSave<IJobDetailTriggerLink>().ToList().ForEach(ScheduleJob);
        }

        void AddJobListeners(IJobDetailJobListenerTriggerLink link) {
            CreateJobListenersInstances((listener, detail) => AddJobListeners(detail, listener), link.JobListenerTrigger.JobType);
        }


        void ScheduleJob(IJobDetailTriggerLink link) {
            var simpleTrigger = Mapper.GetSimpleTrigger(link.JobTrigger, link.JobDetail.Name, link.JobDetail.Group);
            Scheduler.ScheduleJob(simpleTrigger);

        }

        void CreateJobListenersInstances(Action<IJobListener, IJobDetail> action, Type jobType) {
            var type = Application.ObjectSpaceProvider.TypesInfo.FindBussinessObjectType<IJobDetail>();
            var jobDetails = ObjectSpace.GetObjects(type, CriteriaOperator.Parse("JobType=?", jobType)).OfType<IJobDetail>();
            foreach (IJobDetail jobDetail in jobDetails) {
                IJobDetail detail = jobDetail;
                IEnumerable<IJobListener> listeners = GetListeners(detail);
                foreach (var listener in listeners) {
                    action.Invoke(listener, jobDetail);
                }
            }
        }
        void AddJobListeners(IJobDetail jobDetail, IJobListener listener) {
            JobDetail job = Scheduler.GetJobDetail(jobDetail.Name, jobDetail.Group);
            job.AddJobListener(listener.Name);
            Scheduler.AddJobListener(listener);
            Scheduler.Resources.JobStore.StoreJob(Scheduler.SchedulingContext, job, true);
        }

        IEnumerable<IJobListener> GetListeners(IJobDetail jobDetail) {
            var typesInfo = Application.ObjectSpaceProvider.TypesInfo;
            return ReflectionHelper.FindTypeDescendants(typesInfo.FindTypeInfo(typeof(IJobListener))).Where(
                info => info.FindAttribute<JobTypeAttribute>().Type == jobDetail.JobType).Select(
                    typeInfo => typeInfo.CreateInstance()).OfType<IJobListener>();
        }

    }

    internal class JobDetailInfo {
        readonly bool _jobListenerTriggerLink;
        readonly string _triggerName;

        public JobDetailInfo(ISupportJobDetail supportJobDetail) {
            _jobListenerTriggerLink = supportJobDetail is IJobDetailJobListenerTriggerLink;
            if (!_jobListenerTriggerLink) {
                _triggerName = ((IJobDetailTriggerLink) supportJobDetail).JobTrigger.Name;
            }
            JobName = supportJobDetail.JobDetail.Name;
            JobGroup = supportJobDetail.JobDetail.Group;
            JobType = _jobListenerTriggerLink? ((IJobDetailJobListenerTriggerLink) supportJobDetail).JobListenerTrigger.JobType:supportJobDetail.JobDetail.JobType;
        }

        public string TriggerName {
            get { return _triggerName; }
        }

        public bool JobListenerTriggerLink {
            get { return _jobListenerTriggerLink; }
        }

        public string JobName { get; set; }

        public string JobGroup { get; set; }

        public Type JobType { get; set; }
    }
}