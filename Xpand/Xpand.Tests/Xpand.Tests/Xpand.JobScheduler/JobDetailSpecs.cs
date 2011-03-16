using System.Linq;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Quartz;
using Xpand.Persistent.BaseImpl.JobScheduler;
using Xpand.Persistent.BaseImpl.JobScheduler.Triggers;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_new_Job_detail_saved : With_Job_Scheduler_XpandJobDetail_Application<When_new_Job_detail_saved> {
        static JobDetail _jobDetail;
        Because of = () => ObjectSpace.CommitChanges();

        It should_add_a_new_job_detail_to_the_scheduler =
            () => {
                _jobDetail = Scheduler.GetJobDetail(Object);
                _jobDetail.ShouldNotBeNull();
            };

        It should_have_as_group_the_jobtype_plus_the_name_of_the_job_Detail = () => _jobDetail.Group.ShouldEqual(Object.Job.JobType.FullName);

        It should_have_a_new_job_listener = () => {
            Scheduler.GetJobDetail(Object).JobListenerNames.First().ShouldEqual("DummyJobListener");
            Scheduler.GetJobListener("DummyJobListener").ShouldNotBeNull();
        };

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_new_Job_detail_with_group_assigned_saved : With_Job_Scheduler_XpandJobDetail_Application<When_new_Job_detail_with_group_assigned_saved> {
        Establish context = () => {
            var jobSchedulerGroup = ObjectSpace.FindObject<JobSchedulerGroup>(null);
            Object.Group = jobSchedulerGroup;
        };

        protected override void ViewCreated(DetailView detailView) {
            base.ViewCreated(detailView);
            var objectSpace = Application.CreateObjectSpace();
            var jobSchedulerGroup = objectSpace.CreateObject<JobSchedulerGroup>();
            jobSchedulerGroup.Name = "gr";
            var xpandSimpleTrigger = objectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.JobSchedulerGroups.Add(jobSchedulerGroup);
            objectSpace.CommitChanges();
        }
        Because of = () => ObjectSpace.CommitChanges();

        It should_create_triggers_for_that_group = () => Scheduler.GetTriggersOfJob(Object).Count().ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    //TODO:check DB if triggers are removed when job is deleted 
    public class When_Job_detail_Deleted : With_Job_Scheduler_XpandJobDetail_Application<When_Job_detail_Deleted> {

        Establish context = () => {
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.JobDetails.Add(Object);
            ObjectSpace.CommitChanges();
            ObjectSpace.Delete(Object);
        };

        Because of = () => ObjectSpace.CommitChanges();

        It should_remove_it_from_the_scheduler =
            () => Scheduler.GetJobDetail(Object).ShouldBeNull();

        It should_remove_the_listener_from_the_scheduler =
            () => Scheduler.GetTriggerListener("DummyJobListener").ShouldBeNull();
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_Job_detail_updated : With_Job_Scheduler_XpandJobDetail_Application<When_Job_detail_updated> {

        Establish context = () => {
            ObjectSpace.CommitChanges();
            Object.Description = "new_description";
        };

        Because of = () => ObjectSpace.CommitChanges();
        protected override void ViewCreated(DetailView detailView) {
            base.ViewCreated(detailView);
            var xpandJobTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandJobTrigger.Name = "trigger";
            Object.JobTriggers.Add(xpandJobTrigger);
        }

        It should_change_the_value_in_the_scheduler =
            () => Scheduler.GetJobDetail(Object).Description.ShouldEqual("new_description");

        It should_have_the_same_number_of_triggers = () => Scheduler.GetTriggersOfJob(Object).Count().ShouldEqual(1);
        It should_have_the_same_number_of_listeners = () => Scheduler.GetJobDetail(Object).JobListenerNames.Count().ShouldBeGreaterThan(0);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }

    public class When_Job_Detail_is_linked_with_triggers : With_Job_Scheduler_XpandJobDetail_Application<When_Job_Detail_is_linked_with_triggers> {
        Establish context = () => {
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "trigger";
            Object.JobTriggers.Add(xpandSimpleTrigger);
        };


        Because of = () => ObjectSpace.CommitChanges();

        It should_add_one_trigger_to_the_Schedule_job = () => Scheduler.GetTriggersOfJob(Object).Length.ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_Job_Detail_is_unlinked_with_triggers : With_Job_Scheduler_XpandJobDetail_Application<When_Job_Detail_is_unlinked_with_triggers> {

        Establish context = () => {
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "trigger";
            Object.JobTriggers.Add(xpandSimpleTrigger);
            ObjectSpace.CommitChanges();
            xpandSimpleTrigger.Delete();
        };


        Because of = () => ObjectSpace.CommitChanges();

        It should_remove_the_trigger_from_the_schedule_job = () => Scheduler.GetTriggersOfJob(Object).Length.ShouldEqual(0);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
}
