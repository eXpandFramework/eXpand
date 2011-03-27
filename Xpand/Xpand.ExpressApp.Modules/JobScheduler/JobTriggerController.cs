using System;
using System.ComponentModel;
using System.Linq;
using Quartz.Spi;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobTriggerController : SupportSchedulerController {
        public event EventHandler<CancelEventArgs> Committing;

        protected virtual void OnCommitting(CancelEventArgs e) {
            EventHandler<CancelEventArgs> handler = Committing;
            if (handler != null) handler(this, e);
        }
        public JobTriggerController() {
            TargetObjectType = typeof(IXpandJobTrigger);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpaceOnCommitting;
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpaceOnCommitting;
        }
        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            var eventArgs = new CancelEventArgs();
            OnCommitting(eventArgs);
            if (!eventArgs.Cancel)
                ObjectSpace.GetObjectsToUpdate<IXpandJobTrigger>().ToList().ForEach(UpdateTriggers);
        }

        void UpdateTriggers(IXpandJobTrigger obj) {
            StoreTriggers(obj);
            CreateNewTriggers(obj);
        }

        void CreateNewTriggers(IXpandJobTrigger jobTrigger) {
            jobTrigger.JobDetails.Where(detail1 => !Scheduler.HasTriggers(detail1)).Each(CreateTrigger(jobTrigger));
        }

        Action<IXpandJobDetail> CreateTrigger(IXpandJobTrigger jobTrigger) {
            return detail => Scheduler.ScheduleJob(jobTrigger, detail, detail.Group != null ? detail.Group.Name : null);
        }


        void StoreTriggers(IXpandJobTrigger jobTrigger) {
            jobTrigger.JobDetails.Select(detail => Scheduler.GetJobDetail(detail)).Where(detail1 => detail1!=null).ToList().ForEach(
                jobDetail => Scheduler.GetTriggersOfJob(jobDetail.Key).OfType<IOperableTrigger>().ToList().ForEach(
                    StoreTrigger(jobTrigger)));
        }

        Action<IOperableTrigger> StoreTrigger(IXpandJobTrigger jobTrigger) {
            return trigger => {
                trigger.AssignQuartzTrigger(jobTrigger, trigger.JobKey.Name, TypesInfo.FindTypeInfo(trigger.JobKey.Group).Type, null);
                Scheduler.StoreTrigger(trigger);
            };
        }
    }
}
