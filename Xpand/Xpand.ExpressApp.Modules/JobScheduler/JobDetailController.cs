using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using Quartz;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobTriggerController : ViewController {
        public JobTriggerController() {
            TargetObjectType = typeof(IXpandSimpleTrigger);
        }
        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpaceOnCommitting;
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {

        }
    }
    public class JobDetailController : ViewController {
        public JobDetailController() {
            TargetObjectType = typeof(IXpandJobDetail);
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
        void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {
            IScheduler scheduler = Application.FindModule<JobSchedulerModule>().Scheduler;
            objectsManipulatingEventArgs.Objects.OfType<IXpandJobDetail>().ToList().ForEach(detail => scheduler.DeleteJob(detail.Name, detail.Group));
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            IScheduler scheduler = Application.FindModule<JobSchedulerModule>().Scheduler;
            var xpandJobDetail = ((IXpandJobDetail)View.CurrentObject);
            JobDetail jobDetail = GetJobDetail(xpandJobDetail);
            jobDetail.Durable = true;
            scheduler.AddJob(jobDetail, false);

        }

        JobDetail GetJobDetail(IXpandJobDetail xpandJobDetail) {
            return new JobDetail {
                Name = xpandJobDetail.Name,
                Durable = xpandJobDetail.Durable,
                Description = xpandJobDetail.Description,
                Group = xpandJobDetail.Group,
                JobType = xpandJobDetail.JobType,
                RequestsRecovery = xpandJobDetail.RequestsRecovery,
                Volatile = xpandJobDetail.Volatile
            };
        }
    }
}