using System.Linq;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_new_Job_detail_saved : With_Job_Scheduler_XpandJobDetail_Application<When_new_Job_detail_saved> {
        Because of = () => ObjectSpace.CommitChanges();

        It should_add_a_new_job_detail_to_the_scheduler =
            () => Scheduler.GetJobDetail(Object.Name, Object.Group).ShouldNotBeNull();

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }

    public class When_Job_detail_Deleted : With_Job_Scheduler_Application<When_Job_detail_Deleted, XpandJobDetail> {

        Establish context = () => ObjectSpace.CommitChanges();

        Because of = () => ObjectSpace.Delete(Object);


        It should_remove_it_from_the_scheduler =
            () => Scheduler.GetJobDetail(Object.Name, Object.Group).ShouldBeNull();
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_Job_detail_updated : With_Job_Scheduler_XpandJobDetail_Application<When_Job_detail_updated> {

        Establish context = () => {
            ObjectSpace.CommitChanges();
            Object.Group = "changed_group";
        };

        Because of = () => ObjectSpace.CommitChanges();
        protected override void ViewCreated(DetailView detailView) {
            base.ViewCreated(detailView);
            var xpandJobTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandJobTrigger.Name = "trigger";
            Object.JobTriggers.Add(xpandJobTrigger);
        }

        It should_change_the_value_in_the_scheduler =
            () => {
                Scheduler.GetJobDetail(Object.Name, Object.Group).Group.ShouldEqual("changed_group");
                Scheduler.GetJobDetail(Object.Name, "group").ShouldBeNull();
            };

        It should_have_the_same_number_of_triggers = () => Scheduler.GetTriggersOfJob(Object.Name, Object.Group).Count().ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }

    public class When_Job_Detail_is_linked_with_triggers : With_Job_Scheduler_XpandJobDetail_Application<When_Job_Detail_is_linked_with_triggers> {
        Establish context = () => {
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "trigger";
            Object.JobTriggers.Add(xpandSimpleTrigger);
        };


        Because of = () => ObjectSpace.CommitChanges();

        It should_schedule_the_job = () => Scheduler.GetTriggersOfJob(Object.Name, Object.Group).Length.ShouldEqual(1);

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

        It should_schedule_the_job = () => Scheduler.GetTriggersOfJob(Object.Name, Object.Group).Length.ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
}
