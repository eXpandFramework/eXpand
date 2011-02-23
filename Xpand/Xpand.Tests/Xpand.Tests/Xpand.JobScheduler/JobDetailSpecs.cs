using System.Linq;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_new_Job_detail_saved : With_Application<When_new_Job_detail_saved> {
        Because of = () => ObjectSpace.CommitChanges();

        It should_add_a_new_job_detail_to_the_scheduler =
            () => Scheduler.GetJobDetail(JobDetail.Name, JobDetail.Group).ShouldNotBeNull();

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }

    public class When_Job_detail_Deleted : With_Application<When_Job_detail_Deleted> {

        Establish context = () => ObjectSpace.CommitChanges();

        Because of = () => ObjectSpace.Delete(JobDetail);


        It should_remove_it_from_the_scheduler =
            () => Scheduler.GetJobDetail(JobDetail.Name, JobDetail.Group).ShouldBeNull();
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_Job_detail_updated : With_Application<When_Job_detail_updated> {

        Establish context = () => {
            ObjectSpace.CommitChanges();
            JobDetail.Group = "changed_group";
        };

        Because of = () => ObjectSpace.CommitChanges();
        protected override void InitObject(DetailView detailView) {
            base.InitObject(detailView);
            var xpandJobTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandJobTrigger.Name = "trigger";
            JobDetail.JobTriggers.Add(xpandJobTrigger);
        }

        It should_change_the_value_in_the_scheduler =
            () => {
                Scheduler.GetJobDetail(JobDetail.Name, JobDetail.Group).Group.ShouldEqual("changed_group");
                Scheduler.GetJobDetail(JobDetail.Name, "group").ShouldBeNull();
            };

        It should_have_the_same_number_of_triggers = () => Scheduler.GetTriggersOfJob(JobDetail.Name, JobDetail.Group).Count().ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }

    public class When_Job_Detail_is_linked_with_triggers : With_Application<When_Job_Detail_is_linked_with_triggers> {
        Establish context = () => {
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "trigger";
            JobDetail.JobTriggers.Add(xpandSimpleTrigger);
        };


        Because of = () => ObjectSpace.CommitChanges();

        It should_schedule_the_job = () => Scheduler.GetTriggersOfJob(JobDetail.Name, JobDetail.Group).Length.ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_Job_Detail_is_unlinked_with_triggers : With_Application<When_Job_Detail_is_unlinked_with_triggers> {

        Establish context = () => {
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "trigger";
            JobDetail.JobTriggers.Add(xpandSimpleTrigger);
            ObjectSpace.CommitChanges();
            xpandSimpleTrigger.Delete();
        };


        Because of = () => ObjectSpace.CommitChanges();

        It should_schedule_the_job = () => Scheduler.GetTriggersOfJob(JobDetail.Name, JobDetail.Group).Length.ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
}
