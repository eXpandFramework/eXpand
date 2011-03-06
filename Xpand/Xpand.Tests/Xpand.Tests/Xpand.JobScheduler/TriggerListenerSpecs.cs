
using Machine.Specifications;
using Quartz;
using Quartz.Spi;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.JobScheduler;
using Xpand.ExpressApp.JobScheduler.Qaurtz;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Persistent.BaseImpl.JobScheduler.Triggers;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_triggerlistenertrigger_is_linked_with_jobtrigger : With_Job_Scheduler_XpandJobDetail_Application<When_triggerlistenertrigger_is_linked_with_jobtrigger> {
        Establish context = () => {
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "tr";
            xpandSimpleTrigger.JobDetails.Add(Object);
            ObjectSpace.CommitChanges();
            var triggerListenerTrigger = ObjectSpace.CreateObject<TriggerListenerTrigger>();
            triggerListenerTrigger.JobType = Object.Job.JobType;
            xpandSimpleTrigger.TriggerListenerTriggers.Add(triggerListenerTrigger);
        };
        Because of = () => ObjectSpace.CommitChanges();

        It should_set_the_datamap_triggerlisteners_key_value = () => {
            var detail = Scheduler.GetJobDetail(Object);
            var jobData = detail.JobDataMap[XpandScheduler.TriggerTriggerJobListenersOn + TriggerListenerEvent.Fired];
            jobData.ShouldNotBeNull();
            jobData.ShouldEqual(detail.Name + "|" + detail.Group);
        };

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_triggerlistenertrigger_is_unlinked_with_jobtrigger : With_Job_Scheduler_XpandJobDetail_Application<When_triggerlistenertrigger_is_unlinked_with_jobtrigger> {
        Establish context = () => {
            var xpandSimpleTrigger = ObjectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "tr";
            xpandSimpleTrigger.JobDetails.Add(Object);
            ObjectSpace.CommitChanges();
            var triggerListenerTrigger = ObjectSpace.CreateObject<TriggerListenerTrigger>();
            triggerListenerTrigger.JobType = Object.Job.JobType;
            xpandSimpleTrigger.TriggerListenerTriggers.Add(triggerListenerTrigger);
            ObjectSpace.CommitChanges();
            xpandSimpleTrigger.TriggerListenerTriggers.Remove(triggerListenerTrigger);
        };
        Because of = () => ObjectSpace.CommitChanges();

        It should_clear_the_datamap_triggerlisteners_key_value = () => {
            var detail = Scheduler.GetJobDetail(Object);
            var jobData = detail.JobDataMap[XpandScheduler.TriggerTriggerJobListenersOn + TriggerListenerEvent.Fired];
            jobData.ShouldNotBeNull();
            jobData.ShouldEqual(detail.Name + "|" + detail.Group);
        };

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_an_Xpand_TriggerListener_is_executed {
        static IXpandScheduler _scheduler;
        static JobExecutionContext _jobExecutionContext;
        static bool _triggered;
        static XpandTriggerListener _xpandJobListener;

        Establish context = () => {
            _xpandJobListener = new XpandTriggerListener();
            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory();
            _scheduler = (IXpandScheduler)stdSchedulerFactory.GetScheduler();
            _scheduler.Start();
            var jobDetail = new JobDetail { Name = "name", Group = "group" };
            var jobDataMapKeyCalculator = new JobDataMapKeyCalculator();
            jobDataMapKeyCalculator.CreateTriggerListenersKey(jobDetail.JobDataMap, TriggerListenerEvent.Fired, jobDetail.Key);
            var triggerFiredBundle = new TriggerFiredBundle(jobDetail, Isolate.Fake.Instance<Trigger>(), null, false, null, null, null, null);
            _jobExecutionContext = new JobExecutionContext(_scheduler, triggerFiredBundle, null);
            Isolate.WhenCalled(() => _scheduler.TriggerJob(null, null)).DoInstead(callContext => _triggered = true);
        };

        Because of = () => _xpandJobListener.TriggerFired(null,_jobExecutionContext);

        It should_trigger_theJobs_under_TriggerTriggerListenersOnFired = () => _triggered.ShouldBeTrue();
        It should_shutdown_the_scheduler = () => _scheduler.Shutdown(false);
    }

}
