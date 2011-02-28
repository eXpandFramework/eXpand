using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using Quartz;
using Xpand.ExpressApp.JobScheduler.Qaurtz;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobDetailController : SupportSchedulerController {

        readonly List<JobDetail> _jobDetailsToBeDeleted = new List<JobDetail>();

        public JobDetailController() {
            TargetObjectType = typeof(IJobDetail);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleting += ObjectSpaceOnObjectDeleting;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleting -= ObjectSpaceOnObjectDeleting;
        }

        void ObjectSpaceOnObjectDeleting(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {
            _jobDetailsToBeDeleted.AddRange(objectsManipulatingEventArgs.Objects.OfType<IJobDetail>().Select(detail => Scheduler.GetJobDetail(detail)));
        }

        IEnumerable<IJobListener> GetListeners(IJobDetail jobDetail) {
            var jobListeners = ReflectionHelper.FindTypeDescendants(TypesInfo.FindTypeInfo(typeof(IJobListener))).Where(IsRelatedTo(jobDetail)).Select(
                    typeInfo => typeInfo.CreateInstance()).OfType<IJobListener>().ToList();
            jobListeners.Add(new XpandJobListener());
            return jobListeners;
        }

        Func<ITypeInfo, bool> IsRelatedTo(IJobDetail jobDetail) {
            return info => {
                var jobTypeAttribute = info.FindAttribute<JobTypeAttribute>();
                return jobTypeAttribute != null && jobTypeAttribute.Type == jobDetail.Job.JobType;
            };
        }


        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            ObjectSpace.GetNonDeletedObjectsToSave<IJobDetail>().ToList().ForEach(Save);
            _jobDetailsToBeDeleted.ForEach(DeleteFromScheduler);
            _jobDetailsToBeDeleted.Clear();
        }

        void DeleteFromScheduler(JobDetail obj) {
            Scheduler.DeleteJob(obj.Name, obj.Group);
        }

        void AddJobListeners(IJobDetail jobDetail, IJobListener listener) {
            JobDetail job = Scheduler.GetJobDetail(jobDetail);
            job.AddJobListener(listener.Name);
            if (Scheduler.GetJobListener(listener.Name) == null)
                Scheduler.AddJobListener(listener);
            Scheduler.StoreJob(job);
        }

        void Save(IJobDetail detail) {
            Scheduler.StoreJob(detail);
            if (ObjectSpace.IsNewObject(detail)) {
                AddJobListeners(detail);
                CreateTriggers(detail.Group);
            }
        }

        void CreateTriggers(IJobSchedulerGroup jobSchedulerGroup) {
            if (jobSchedulerGroup != null) {
                var objects = ObjectSpace.GetObjects(TypesInfo.FindBussinessObjectType<IJobTrigger>(), ForTheSameGroup(jobSchedulerGroup)).OfType<IJobTrigger>().ToList();
                objects.ForEach(ScheduleJob);
            }
        }

        CriteriaOperator ForTheSameGroup(IJobSchedulerGroup jobSchedulerGroup) {
            return CriteriaOperator.Parse("JobSchedulerGroups[Name=?]", jobSchedulerGroup.Name);
        }

        void ScheduleJob(IJobTrigger trigger) {
            var jobDetail = View.CurrentObject as IJobDetail;
            if (jobDetail != null) {
                var simpleTrigger = Mapper.GetSimpleTrigger(trigger, jobDetail.Name, jobDetail.Job.JobType, jobDetail.Group.Name);
                Scheduler.ScheduleJob(simpleTrigger);
            }
        }

        void AddJobListeners(IJobDetail detail) {
            GetListeners(detail).ToList().ForEach(listener => AddJobListeners(detail, listener));
        }
    }
}