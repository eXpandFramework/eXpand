using System;
using System.Threading;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Xpand.ExpressApp.JobScheduler;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_trigger_time_pass : With_Scheduler {

        Establish context = () => {
            
            var objectSpace = ObjectSpaceInMemory.CreateNew();
            var xpandSimpleTrigger = objectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "tr";
            var simpleTrigger = Mapper.CreateTrigger(xpandSimpleTrigger, "jb", typeof(DummyJob),null);

            simpleTrigger.StartTimeUtc = DateTime.UtcNow;
            
            Scheduler.ScheduleJob(simpleTrigger);
            Scheduler.Start();
        };

        Because of = () => Thread.Sleep(5000);

        It should_execute_the_job = () => JobExecutedCount.ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }

}
