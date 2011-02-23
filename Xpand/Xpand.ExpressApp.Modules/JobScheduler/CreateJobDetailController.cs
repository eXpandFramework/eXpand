using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using Quartz;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    public class CreateJobDetailController : ViewController {
        JobDetail _currentJobDetail;

        public CreateJobDetailController() {
            TargetObjectType = typeof(IJobDetail);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleted += ObjectSpaceOnObjectDeleted;
            View.CurrentObjectChanged+=ViewOnCurrentObjectChanged;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleted -= ObjectSpaceOnObjectDeleted;
            View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
        }

        void ViewOnCurrentObjectChanged(object sender, EventArgs eventArgs) {
            IScheduler scheduler = Application.FindModule<JobSchedulerModule>().Scheduler;
            var detail = ((IJobDetail) View.CurrentObject);
            _currentJobDetail = scheduler.GetJobDetail(detail.Name, detail.Group);
        }

        void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {
            IScheduler scheduler = Application.FindModule<JobSchedulerModule>().Scheduler;
            objectsManipulatingEventArgs.Objects.OfType<IJobDetail>().ToList().ForEach(detail => scheduler.DeleteJob(detail.Name, detail.Group));
        }
        
        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            IScheduler scheduler = Application.FindModule<JobSchedulerModule>().Scheduler;
            var xpandJobDetail = ((IJobDetail)View.CurrentObject);
            if (ObjectSpace.IsNewObject(xpandJobDetail)) {
                AddJob(xpandJobDetail, scheduler);
            }
            else {

                var triggers = scheduler.GetTriggersOfJob(_currentJobDetail.Name, _currentJobDetail.Group).ToList();
                scheduler.DeleteJob(_currentJobDetail.Name, _currentJobDetail.Group);
                AddJob(xpandJobDetail, scheduler);
                foreach (var trigger in triggers) {
                    trigger.JobGroup = xpandJobDetail.Group;
                    scheduler.ScheduleJob(trigger);
                }
            }
            _currentJobDetail = scheduler.GetJobDetail(xpandJobDetail.Name, xpandJobDetail.Group);
        }

        void AddJob(IJobDetail xpandJobDetail, IScheduler scheduler) {
            JobDetail jobDetail = Mapper.GetJobDetail(xpandJobDetail);
            jobDetail.Durable = true;
            scheduler.AddJob(jobDetail, false);
        }
    }
}