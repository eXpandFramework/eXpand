using System;
using DevExpress.ExpressApp;
using Quartz;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.JobScheduler;
using System.Linq;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobSchedulerController : ViewController {
        public JobSchedulerController() {
            TargetObjectType = typeof(IJobDetail);
        }
        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.ObjectSaving += ObjectSpaceOnObjectSaving;
            ObjectSpace.ObjectDeleting += ObjectSpaceOnObjectDeleting;
        }

        void ObjectSpaceOnObjectDeleting(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {
            foreach (IJobDetailTriggerLink jobDetailTriggerLink in objectsManipulatingEventArgs.Objects.OfType<IJobDetailTriggerLink>()) {
                DoSchedulerAction(jobDetailTriggerLink,(scheduler, detail, trigger) =>scheduler.UnscheduleJob( trigger.Name,trigger.JobGroup));
            }
        }

        void DoSchedulerAction(IJobDetailTriggerLink jobDetailTriggerLink, Action<IScheduler,JobDetail, Trigger> action) {
            IScheduler scheduler = Application.FindModule<JobSchedulerModule>().Scheduler;
            JobDetail jobDetail = scheduler.GetJobDetail(jobDetailTriggerLink.JobDetail.Name, jobDetailTriggerLink.JobDetail.Group);
            SimpleTrigger simpleTrigger = Mapper.GetSimpleTrigger(jobDetailTriggerLink.JobTrigger,jobDetail.Name, jobDetail.Group);
            action.Invoke(scheduler, jobDetail, simpleTrigger);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.ObjectSaving -= ObjectSpaceOnObjectSaving;
            ObjectSpace.ObjectDeleting -= ObjectSpaceOnObjectDeleting;
        }
        void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs objectManipulatingEventArgs) {
            if (objectManipulatingEventArgs.Object is IJobDetailTriggerLink) {
                var link = (IJobDetailTriggerLink) objectManipulatingEventArgs.Object;
                DoSchedulerAction(link, (scheduler, detail, trigger) => scheduler.ScheduleJob(trigger));
            }
        }
    }
}