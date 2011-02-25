using System;
using System.ComponentModel;
using System.Linq;
using Quartz;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobTriggerController : SupportSchedulerController {
        public JobTriggerController() {
            TargetObjectType = typeof(ISimpleTrigger);
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
            ObjectSpace.GetObjectsToUpdate<ISimpleTrigger>().ToList().ForEach(UpdateTriggers);
        }

        void UpdateTriggers(ISimpleTrigger obj) {
            obj.JobDetails.Select(detail => Scheduler.GetJobDetail(detail)).ToList().ForEach(
                jobDetail =>Scheduler.GetTriggersOfJob(jobDetail.Name, jobDetail.Group).OfType<SimpleTrigger>().ToList().ForEach(
                    Update(obj)));
        }

        Action<SimpleTrigger> Update(ISimpleTrigger obj) {
            return trigger => {
                Mapper.AssignTrigger(trigger, obj, trigger.JobName, TypesInfo.FindTypeInfo(trigger.JobGroup).Type);
                Scheduler.StoreTrigger(trigger);
            };
        }
    }
}
