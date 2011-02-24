using System;
using System.ComponentModel;
using System.Linq;
using Quartz;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobTriggerController : SupportSchedulerController {
        public JobTriggerController() {
            TargetObjectType = typeof(IXpandSimpleTrigger);
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
            ObjectSpace.GetObjectsToUpdate<IXpandSimpleTrigger>().ToList().ForEach(UpdateTriggers);
        }

        void UpdateTriggers(IXpandSimpleTrigger obj) {
            obj.JobDetails.Select(detail => Scheduler.GetJobDetail(detail)).ToList().ForEach(
                jobDetail =>Scheduler.GetTriggersOfJob(jobDetail.Name, jobDetail.Group).OfType<SimpleTrigger>().ToList().ForEach(
                    Update(obj)));
        }

        Action<SimpleTrigger> Update(IXpandSimpleTrigger obj) {
            return trigger => {
                Mapper.AssignTrigger(trigger, obj, trigger.JobName, trigger.JobGroup);
                Scheduler.Resources.JobStore.StoreTrigger(Scheduler.SchedulingContext, trigger, false);
            };
        }
    }
}
