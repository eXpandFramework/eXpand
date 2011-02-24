using System;
using System.Linq;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Quartz;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_JobTrigger_is_linked_with_jobdetail : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_is_linked_with_jobdetail> {


        Establish context = () => {
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "trigger";
            xpandSimpleTrigger.JobDetails.Add(Object);
        };


        Because of = () => ObjectSpace.CommitChanges();

        It should_schedule_the_job = () => Scheduler.GetTriggersOfJob(Object.Name, Object.Group).Length.ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }

    internal class MyClass {
        Establish context = () => {
            objectSpace = ObjectSpaceInMemory.CreateNew();
            var xpandJobTrigger = objectSpace.CreateObject<XpandSimpleTrigger>();
            xpandJobTrigger.JobDetails.Add(objectSpace.CreateObject<XpandJobDetail>());
            objectSpace.CommitChanges();
            objectSpace.ObjectDeleting += (sender, args) => _count = args.Objects.OfType<JobDetailTriggerLink>().Count();
            objectSpace.Delete(xpandJobTrigger);
        };

        Because of = () => objectSpace.CommitChanges();
        It should_should = () => _count.ShouldBeGreaterThan(0);
        static int _count;
        static IObjectSpace objectSpace;
    }
    public class When_JobTrigger_is_deleted : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_is_deleted> {
        static IObjectSpace _objectSpace;

        Establish context = () => {

            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "trigger";
            xpandSimpleTrigger.JobDetails.Add(Object);
            ObjectSpace.CommitChanges();
            Scheduler.GetTriggersOfJob(Object.Name, Object.Group).Count().ShouldEqual(1);
            _objectSpace = Application.CreateObjectSpace();
            xpandSimpleTrigger = _objectSpace.GetObject(xpandSimpleTrigger);
            var detailView = Application.CreateDetailView(_objectSpace, xpandSimpleTrigger);
            Window.SetView(detailView);
            _objectSpace.Delete(xpandSimpleTrigger);
        };

        protected override System.Collections.Generic.IList<System.Type> GetDomaincomponentTypes() {
            var domaincomponentTypes = base.GetDomaincomponentTypes();
            domaincomponentTypes.Add(typeof(XpandSimpleTrigger));
            return domaincomponentTypes;
        }

        Because of = () => _objectSpace.CommitChanges();

        It should_remove_the_trigger_from_the_scheduler_jobs =
            () => Scheduler.GetTriggersOfJob(Object.Name, Object.Group).Count().ShouldEqual(0);
    }
    public class When_JobTrigger_is_updated : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_is_updated> {
        static IObjectSpace _objectSpace;

        Establish context = () => {
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "trigger";
            xpandSimpleTrigger.JobDetails.Add(Object);
            ObjectSpace.CommitChanges();
            Scheduler.GetTriggersOfJob(Object.Name, Object.Group).Count().ShouldEqual(1);
            _objectSpace = Application.CreateObjectSpace();
            xpandSimpleTrigger = _objectSpace.GetObject(xpandSimpleTrigger);
            var detailView = Application.CreateDetailView(_objectSpace, xpandSimpleTrigger);
            Window.SetView(detailView);
            xpandSimpleTrigger.StartTimeUtc=DateTime.MaxValue;
            
        };

        protected override System.Collections.Generic.IList<System.Type> GetDomaincomponentTypes() {
            var domaincomponentTypes = base.GetDomaincomponentTypes();
            domaincomponentTypes.Add(typeof(XpandSimpleTrigger));
            return domaincomponentTypes;
        }

        Because of = () => _objectSpace.CommitChanges();

        It should_remove_the_trigger_from_the_scheduler_jobs =
            () =>
            Scheduler.GetTriggersOfJob(Object.Name, Object.Group).OfType<SimpleTrigger>().First().StartTimeUtc.
                ShouldEqual(DateTime.MaxValue);
    }
}
