using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Quartz;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobDetailController : SupportSchedulerController {

        readonly List<IJobDetail> _jobDetailsToBeDeleted = new List<IJobDetail>();

        public JobDetailController() {
            TargetObjectType = typeof(IXpandJobDetail);
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
            _jobDetailsToBeDeleted.AddRange(objectsManipulatingEventArgs.Objects.OfType<IXpandJobDetail>().Select(detail => Scheduler.GetJobDetail(detail)).Where(jobDetail => jobDetail != null));
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            ObjectSpace.GetNonDeletedObjectsToSave<IXpandJobDetail>().ToList().ForEach(Save);
            _jobDetailsToBeDeleted.ForEach(DeleteFromScheduler);
            _jobDetailsToBeDeleted.Clear();
        }

        void DeleteFromScheduler(IJobDetail obj) {
            Scheduler.DeleteJob(new JobKey(obj.Key.Name, obj.Key.Group));
        }


        void Save(IXpandJobDetail detail) {
            Scheduler.StoreJob(detail);
            if (ObjectSpace.IsNewObject(detail)) {
                IJobDetail job = Scheduler.GetJobDetail(detail);
                Scheduler.StoreJob(job);
                CreateTriggers(detail.Group);
            }
        }

        void CreateTriggers(IJobSchedulerGroup jobSchedulerGroup) {
            if (jobSchedulerGroup != null) {
                var objects = ObjectSpace.GetObjects(TypesInfo.FindBussinessObjectType<IXpandJobTrigger>(), ForTheSameGroup(jobSchedulerGroup)).OfType<IXpandJobTrigger>().ToList();
                objects.ForEach(ScheduleJob);
            }
        }

        CriteriaOperator ForTheSameGroup(IJobSchedulerGroup jobSchedulerGroup) {
            return CriteriaOperator.Parse("JobSchedulerGroups[Name=?]", jobSchedulerGroup.Name);
        }

        void ScheduleJob(IXpandJobTrigger trigger) {
            var jobDetail = View.CurrentObject as IXpandJobDetail;
            if (jobDetail != null) {
                Scheduler.ScheduleJob(trigger, jobDetail, jobDetail.Group.Name);
            }
        }

    }
}