using System;
using System.Linq;
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Machine.Specifications;
using Quartz;
using Xpand.Persistent.BaseImpl.JobScheduler;
using Xpand.Persistent.BaseImpl.JobScheduler.Triggers;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_JobTrigger_is_linked_with_jobdetail : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_is_linked_with_jobdetail> {
        static Trigger[] _triggersOfJob;
        static XpandSimpleTrigger _xpandSimpleTrigger;

        Establish context = () => {
            _xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            _xpandSimpleTrigger.Name = "trigger";
            _xpandSimpleTrigger.JobDetails.Add(Object);
        };

        Because of = () => ObjectSpace.CommitChanges();

        It should_schedule_the_job = () => {
            _triggersOfJob = Scheduler.GetTriggersOfJob(Object);
            _triggersOfJob.Length.ShouldEqual(1);
        };

        It should_calculate_the_FinalFireTimeUtc = () => _xpandSimpleTrigger.FinalFireTimeUtc.ShouldNotBeNull();

        It should_add_an_xpandtriggerListerner_to_it = () => _triggersOfJob[0].TriggerListenerNames.Count().ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }

    public class When_2_jobtriggers_are_linked_with_jobdetail : With_Job_Scheduler_XpandJobDetail_Application<When_2_jobtriggers_are_linked_with_jobdetail> {
        Establish context = () => {
            var jobDetail = ObjectSpace.CreateObject<XpandJobDetail>();
            jobDetail.Name = "jb2";
            jobDetail.Job = ObjectSpace.CreateObject<XpandJob>();
            jobDetail.Job.JobType = typeof(DummyJob);
            jobDetail.JobDetailDataMap = ObjectSpace.CreateObject<DummyDetailDataMap>();
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "trigger";
            xpandSimpleTrigger.JobDetails.Add(Object);
            xpandSimpleTrigger.JobDetails.Add(jobDetail);
        };
        Because of = () => ObjectSpace.CommitChanges();

        It should_add_one_trigger_with_trigger_group_same_as_first_jobdetail_group_plus_name =
            () => Scheduler.GetTrigger("trigger", typeof(DummyJob).FullName + "." + Object.Name).ShouldNotBeNull();
        It should_add_one_trigger_with_trigger_group_same_as_second_jobdetail_group_plus_name =
            () => Scheduler.GetTrigger("trigger", typeof(DummyJob).FullName + ".jb2").ShouldNotBeNull();
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_JobTrigger_is_deleted : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_is_deleted> {
        static IObjectSpace _objectSpace;

        protected override System.Collections.Generic.IList<Type> GetDomaincomponentTypes() {
            var domaincomponentTypes = base.GetDomaincomponentTypes();
            domaincomponentTypes.Add(typeof(XpandSimpleTrigger));
            return domaincomponentTypes;
        }

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

        Because of = () => _objectSpace.CommitChanges();

        It should_remove_the_trigger_from_the_scheduler_jobs =
            () => Scheduler.GetTriggersOfJob(Object).Count().ShouldEqual(0);
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_JobTrigger_is_updated : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_is_updated> {
        static XpandSimpleTrigger _xpandSimpleTrigger;
        static IObjectSpace _objectSpace;

        protected override System.Collections.Generic.IList<Type> GetDomaincomponentTypes() {
            var domaincomponentTypes = base.GetDomaincomponentTypes();
            domaincomponentTypes.Add(typeof(XpandSimpleTrigger));
            return domaincomponentTypes;
        }

        Establish context = () => {
            _xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            _xpandSimpleTrigger.Name = "trigger";
            _xpandSimpleTrigger.JobDetails.Add(Object);
            ObjectSpace.CommitChanges();
            _objectSpace = Application.CreateObjectSpace();
            _xpandSimpleTrigger = _objectSpace.GetObject(_xpandSimpleTrigger);
            var detailView = Application.CreateDetailView(_objectSpace, _xpandSimpleTrigger);
            Window.SetView(detailView);
            _xpandSimpleTrigger.Description = "test";

        };

        Because of = () => _objectSpace.CommitChanges();

        It should_update_the_trigger = () => Scheduler.GetTriggersOfJob(Object).OfType<SimpleTrigger>().First().Description.ShouldEqual("test");

        It should_calculate_the_FinalFireTimeUtc = () => _xpandSimpleTrigger.FinalFireTimeUtc.ShouldNotBeNull();
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }

    public class When_JobTrigger_that_does_not_exist_in_the_scheduler_db_gets_updated : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_that_does_not_exist_in_the_scheduler_db_gets_updated> {
        static XpandSimpleTrigger _xpandSimpleTrigger;
        static IObjectSpace _objectSpace;
        protected override System.Collections.Generic.IList<Type> GetDomaincomponentTypes() {
            var domaincomponentTypes = base.GetDomaincomponentTypes();
            domaincomponentTypes.Add(typeof(XpandSimpleTrigger));
            return domaincomponentTypes;
        }
        Establish context = () => {
            _xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            _xpandSimpleTrigger.Name = "trigger";
            _xpandSimpleTrigger.JobDetails.Add(Object);
            ObjectSpace.CommitChanges();
            Scheduler.Start();
            Scheduler.TriggerJob(Object);
            Thread.Sleep(1000);
            _objectSpace = Application.CreateObjectSpace();
            _xpandSimpleTrigger = _objectSpace.GetObject(_xpandSimpleTrigger);
            var detailView = Application.CreateDetailView(_objectSpace, _xpandSimpleTrigger);
            Window.SetView(detailView);
            _xpandSimpleTrigger.Description = "test";
        };
        Because of = () => _objectSpace.CommitChanges();
        It should_update_the_trigger = () => Scheduler.GetTriggersOfJob(Object).OfType<SimpleTrigger>().First().Description.ShouldEqual("test");

        It should_calculate_the_FinalFireTimeUtc = () => _xpandSimpleTrigger.FinalFireTimeUtc.ShouldNotBeNull();
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_JobTrigger_is_linked_with_a_group_that_has_2jobs : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_is_linked_with_a_group_that_has_2jobs> {
        static XpandJobDetail _xpandJobDetail;

        protected override System.Collections.Generic.IList<Type> GetDomaincomponentTypes() {
            var domaincomponentTypes = base.GetDomaincomponentTypes();
            domaincomponentTypes.Add(typeof(XpandSimpleTrigger));
            return domaincomponentTypes;
        }

        Establish context = () => {
            var jobSchedulerGroup = ObjectSpace.CreateObject<JobSchedulerGroup>();
            jobSchedulerGroup.Name = "gr";
            Object.Group = jobSchedulerGroup;
            ObjectSpace.CommitChanges();
            _xpandJobDetail = (XpandJobDetail)new Cloner().CloneTo(Object, typeof(XpandJobDetail));
            _xpandJobDetail.Name = "jb1";
            DetailView.CurrentObject = _xpandJobDetail;
            ObjectSpace.CommitChanges();
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            DetailView.CurrentObject = xpandSimpleTrigger;
            xpandSimpleTrigger.Name = "trigger";
            xpandSimpleTrigger.JobSchedulerGroups.Add(jobSchedulerGroup);
        };

        Because of = () => ObjectSpace.CommitChanges();

        It should_schedule_a_trigger_for_job1 = () => Scheduler.GetTriggersOfJob(Object).Count().ShouldEqual(1);
        It should_schedule_a_trigger_for_job2 = () => Scheduler.GetTriggersOfJob(_xpandJobDetail).Count().ShouldEqual(1);
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_JobTrigger_is_unlinked_with_a_group_that_has_2jobs : With_Job_Scheduler_XpandJobDetail_Application<When_JobTrigger_is_unlinked_with_a_group_that_has_2jobs> {
        static XpandJobDetail _xpandJobDetail;

        protected override System.Collections.Generic.IList<Type> GetDomaincomponentTypes() {
            var domaincomponentTypes = base.GetDomaincomponentTypes();
            domaincomponentTypes.Add(typeof(XpandSimpleTrigger));
            return domaincomponentTypes;
        }

        Establish context = () => {
            var jobSchedulerGroup = ObjectSpace.CreateObject<JobSchedulerGroup>();
            jobSchedulerGroup.Name = "gr";
            Object.Group = jobSchedulerGroup;
            ObjectSpace.CommitChanges();
            _xpandJobDetail = (XpandJobDetail)new Cloner().CloneTo(Object, typeof(XpandJobDetail));
            _xpandJobDetail.Name = "jb1";
            DetailView.CurrentObject = _xpandJobDetail;
            ObjectSpace.CommitChanges();
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            DetailView.CurrentObject = xpandSimpleTrigger;
            xpandSimpleTrigger.Name = "trigger";
            xpandSimpleTrigger.JobSchedulerGroups.Add(jobSchedulerGroup);
            ObjectSpace.CommitChanges();
            xpandSimpleTrigger.JobSchedulerGroups.Remove(jobSchedulerGroup);
        };

        Because of = () => ObjectSpace.CommitChanges();

        It should_schedule_a_trigger_for_job1 = () => Scheduler.GetTriggersOfJob(Object).Count().ShouldEqual(0);
        It should_schedule_a_trigger_for_job2 = () => Scheduler.GetTriggersOfJob(_xpandJobDetail).Count().ShouldEqual(0);
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
}
