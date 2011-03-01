using System;
using System.ComponentModel;
using System.Linq;
using Quartz;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobTriggerController : SupportSchedulerController {
        bool _refreshingLinks;

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
            RescheduleJobs(obj);
            CreateNewTriggers(obj);
        }

        void CreateNewTriggers(IJobTrigger obj) {
            if (!_refreshingLinks) {
                _refreshingLinks = true;
                var jobDetails = obj.JobDetails.Where(detail1 => !Scheduler.HasTriggers(detail1)).ToList();
                jobDetails.ForEach(detail => obj.JobDetails.Remove(detail));
                ObjectSpace.CommitChanges();
                jobDetails.ForEach(jobDetail => obj.JobDetails.Add(jobDetail));
                ObjectSpace.CommitChanges();
                _refreshingLinks = false;
            }
        }

        void RescheduleJobs(IJobTrigger obj) {
            obj.JobDetails.Select(detail => Scheduler.GetJobDetail(detail)).ToList().ForEach(
                jobDetail => Scheduler.GetTriggersOfJob(jobDetail.Name, jobDetail.Group).OfType<Trigger>().ToList().ForEach(
                    RescheduleJob(obj)));
        }

        Action<Trigger> RescheduleJob(IJobTrigger obj) {
            return trigger => {
                Mapper.AssignQuartzTrigger(trigger, obj, trigger.JobName, TypesInfo.FindTypeInfo(trigger.JobGroup).Type, null);
                Scheduler.RescheduleJob(trigger);
            };
        }
    }
}
