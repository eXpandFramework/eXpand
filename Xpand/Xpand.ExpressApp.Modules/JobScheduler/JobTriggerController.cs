using System;
using System.ComponentModel;
using System.Linq;
using Quartz;
using Xpand.ExpressApp.JobScheduler.Qaurtz;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobTriggerController : SupportSchedulerController {
        public JobTriggerController() {
            TargetObjectType = typeof(IJobTrigger);
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
            ObjectSpace.GetObjectsToUpdate<IJobTrigger>().ToList().ForEach(UpdateTriggers);
        }

        void UpdateTriggers(IJobTrigger obj) {
            StoreTriggers(obj);
            CreateNewTriggers(obj);
        }

        void CreateNewTriggers(IJobTrigger jobTrigger) {
            jobTrigger.JobDetails.Where(detail1 => !Scheduler.HasTriggers(detail1)).Each(CreateTrigger(jobTrigger));
        }

        Action<IJobDetail> CreateTrigger(IJobTrigger jobTrigger) {
            return detail => Scheduler.ScheduleJob(jobTrigger, detail, detail.Group != null ? detail.Group.Name : null);
        }


        void StoreTriggers(IJobTrigger jobTrigger) {
            jobTrigger.JobDetails.Select(detail => Scheduler.GetJobDetail(detail)).ToList().ForEach(
                jobDetail => Scheduler.GetTriggersOfJob(jobDetail.Name, jobDetail.Group).OfType<Trigger>().ToList().ForEach(
                    StoreTrigger(jobTrigger)));
        }

        Action<Trigger> StoreTrigger(IJobTrigger jobTrigger) {
            return trigger => {
                trigger.AssignQuartzTrigger(jobTrigger, trigger.JobName, TypesInfo.FindTypeInfo(trigger.JobGroup).Type, null);
                Scheduler.StoreTrigger(trigger);
            };
        }
    }
}
