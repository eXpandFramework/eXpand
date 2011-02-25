using System;
using System.Linq;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Quartz;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_JobTrigger_is_linked_with_jobdetail : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_is_linked_with_jobdetail> {
        static XpandSimpleTrigger _xpandSimpleTrigger;

        Establish context = () => {
            _xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            _xpandSimpleTrigger.Name = "trigger";
            _xpandSimpleTrigger.JobDetails.Add(Object);
        };

        Because of = () => ObjectSpace.CommitChanges();

        It should_schedule_the_job = () => Scheduler.GetTriggersOfJob(Object).Length.ShouldEqual(1);

        It should_calculate_the_FinalFireTimeUtc = () => _xpandSimpleTrigger.FinalFireTimeUtc.ShouldNotBeNull();

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);


    }

    public class When_JobTrigger_is_deleted : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_is_deleted> {
        static IObjectSpace _objectSpace;

        Establish context = () => {

            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "trigger";
            xpandSimpleTrigger.JobDetails.Add(Object);
            ObjectSpace.CommitChanges();
            Scheduler.GetTriggersOfJob(Object).Count().ShouldEqual(1);
            _objectSpace = Application.CreateObjectSpace();
            xpandSimpleTrigger = _objectSpace.GetObject(xpandSimpleTrigger);
            var detailView = Application.CreateDetailView(_objectSpace, xpandSimpleTrigger);
            Window.SetView(detailView);
            _objectSpace.Delete(xpandSimpleTrigger);
        };

        protected override System.Collections.Generic.IList<Type> GetDomaincomponentTypes() {
            var domaincomponentTypes = base.GetDomaincomponentTypes();
            domaincomponentTypes.Add(typeof(XpandSimpleTrigger));
            return domaincomponentTypes;
        }

        Because of = () => _objectSpace.CommitChanges();

        It should_remove_the_trigger_from_the_scheduler_jobs =
            () => Scheduler.GetTriggersOfJob(Object).Count().ShouldEqual(0);
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_JobTrigger_is_updated : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_is_updated> {
        static XpandSimpleTrigger _xpandSimpleTrigger;
        static IObjectSpace _objectSpace;

        Establish context = () => {
            _xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            _xpandSimpleTrigger.Name = "trigger";
            _xpandSimpleTrigger.JobDetails.Add(Object);
            ObjectSpace.CommitChanges();
            Scheduler.GetTriggersOfJob(Object).Count().ShouldEqual(1);
            _objectSpace = Application.CreateObjectSpace();
            _xpandSimpleTrigger = _objectSpace.GetObject(_xpandSimpleTrigger);
            var detailView = Application.CreateDetailView(_objectSpace, _xpandSimpleTrigger);
            Window.SetView(detailView);
            _xpandSimpleTrigger.Description = "test";

        };

        protected override System.Collections.Generic.IList<Type> GetDomaincomponentTypes() {
            var domaincomponentTypes = base.GetDomaincomponentTypes();
            domaincomponentTypes.Add(typeof(XpandSimpleTrigger));
            return domaincomponentTypes;
        }

        Because of = () => _objectSpace.CommitChanges();

        It should_remove_the_trigger_from_the_scheduler_jobs =
            () =>
            Scheduler.GetTriggersOfJob(Object).OfType<SimpleTrigger>().First().Description.
                ShouldEqual("test");

        It should_calculate_the_FinalFireTimeUtc = () => _xpandSimpleTrigger.FinalFireTimeUtc.ShouldNotBeNull();
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
}
