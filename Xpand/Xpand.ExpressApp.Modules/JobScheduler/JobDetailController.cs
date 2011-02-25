using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using Quartz;
using Xpand.ExpressApp.JobScheduler.Qaurtz;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobDetailController : SupportSchedulerController {

        public JobDetailController() {
            TargetObjectType = typeof(IJobDetail);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleted += ObjectSpaceOnObjectDeleted;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleted -= ObjectSpaceOnObjectDeleted;
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
                return jobTypeAttribute != null && jobTypeAttribute.Type == jobDetail.JobType;
            };
        }

        void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {
            objectsManipulatingEventArgs.Objects.OfType<IJobDetail>().ToList().ForEach(jobDetail => Scheduler.DeleteJob(jobDetail));
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            var jobDetail = ((IJobDetail)View.CurrentObject);
            if (jobDetail != null) {
                ObjectSpace.GetNonDeletedObjectsToSave<IJobDetail>().ToList().ForEach(Save);
            }
        }
        void AddJobListeners(IJobDetail jobDetail, IJobListener listener) {
            JobDetail job = Scheduler.GetJobDetail(jobDetail);
            job.AddJobListener(listener.Name);
            if (Scheduler.GetJobListener(listener.Name)==null)
                Scheduler.AddJobListener(listener);
            Scheduler.StoreJob(job);
        }

        void Save(IJobDetail detail) {
            Scheduler.StoreJob(detail);
            if (ObjectSpace.IsNewObject(detail))
                GetListeners(detail).ToList().ForEach(listener => AddJobListeners(detail, listener));
        }
    }
}