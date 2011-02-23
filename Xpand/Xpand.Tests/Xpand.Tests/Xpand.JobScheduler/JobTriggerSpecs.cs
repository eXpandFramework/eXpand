using Machine.Specifications;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_JobTrigger_is_linked_with_jobdetail : With_Application<When_JobTrigger_is_linked_with_jobdetail> {


        Establish context = () => {
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "trigger";
            xpandSimpleTrigger.JobDetails.Add(JobDetail);
        };


        Because of = () => ObjectSpace.CommitChanges();

        It should_schedule_the_job = () => Scheduler.GetTriggersOfJob(JobDetail.Name, JobDetail.Group).Length.ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
}
