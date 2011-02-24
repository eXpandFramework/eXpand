using Machine.Specifications;
using Xpand.Persistent.BaseImpl.JobScheduler;
using System.Linq;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_JobListenerTrigger_is_linked_with_jobdetail : With_Application<When_JobListenerTrigger_is_linked_with_jobdetail> {
        static string _listenerName;

        Establish context = () => {
            ObjectSpace.CommitChanges();
            var xpandJobDetail = ObjectSpace.CreateObject<XpandJobDetail>();
            xpandJobDetail.JobType = typeof (DummyJob2);
            xpandJobDetail.Group = "group";
            xpandJobDetail.Name = "name2";
            var jobListenerTrigger = ObjectSpace.CreateObject<JobListenerTrigger>();
            jobListenerTrigger.JobType = typeof (DummyJob);
            xpandJobDetail.JobListenerTriggers.Add(jobListenerTrigger);
            DetailView.CurrentObject = xpandJobDetail;
        };

        Because of = () => ObjectSpace.CommitChanges();

        It should_be_added_in_jobdetail_listeners_collection = () => {
            _listenerName = Scheduler.GetJobDetail(JobDetail.Name, JobDetail.Group).JobListenerNames.FirstOrDefault();
            _listenerName.ShouldNotBeNull();
        };

        It should_be_added_to_scheduler_listener_collection =
            () => Scheduler.GetJobListener(_listenerName).ShouldNotBeNull();
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_JobListenerTrigger_is_unlinked_with_jobdetail : With_Application<When_JobListenerTrigger_is_linked_with_jobdetail> {

        Establish context = () => {
            ObjectSpace.CommitChanges();
            var xpandJobDetail = ObjectSpace.CreateObject<XpandJobDetail>();
            xpandJobDetail.JobType = typeof (DummyJob2);
            xpandJobDetail.Group = "group";
            xpandJobDetail.Name = "name2";
            var jobListenerTrigger = ObjectSpace.CreateObject<JobListenerTrigger>();
            jobListenerTrigger.JobType = typeof (DummyJob);
            xpandJobDetail.JobListenerTriggers.Add(jobListenerTrigger);
            DetailView.CurrentObject = xpandJobDetail;
            ObjectSpace.CommitChanges();
            ObjectSpace.Delete(jobListenerTrigger);
        };

        Because of = () => ObjectSpace.CommitChanges();
        It should_be_removed_from_job_listeners_collection = () => Scheduler.GetJobDetail(JobDetail.Name, JobDetail.Group).JobListenerNames.FirstOrDefault().ShouldBeNull();

        It should_be_removed_from_scheduler_listeners_collection =
            () => Scheduler.JobListenerNames.Count.ShouldEqual(0);
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }

    public class When_a_job_with_a_listener_is_executed : With_Application<When_a_job_with_a_listener_is_executed> {
        Establish context = () => {
            ObjectSpace.CommitChanges();
            var xpandJobDetail = ObjectSpace.CreateObject<XpandJobDetail>();
            xpandJobDetail.JobType = typeof(DummyJob2);
            xpandJobDetail.Group = "group";
            xpandJobDetail.Name = "name2";
            var jobListenerTrigger = ObjectSpace.CreateObject<JobListenerTrigger>();
            jobListenerTrigger.JobType = typeof(DummyJob);
            xpandJobDetail.JobListenerTriggers.Add(jobListenerTrigger);
            DetailView.CurrentObject = xpandJobDetail;
            ObjectSpace.CommitChanges();
        };

        Because of = () => Scheduler.TriggerJob(JobDetail);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
}
