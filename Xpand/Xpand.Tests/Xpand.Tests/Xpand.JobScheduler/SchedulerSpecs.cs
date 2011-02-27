using System;
using System.Threading;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Quartz;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.JobScheduler;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_trigger_time_pass : With_Scheduler {

        Establish context = () => {
            
            var objectSpace = ObjectSpaceInMemory.CreateNew();
            var xpandSimpleTrigger = objectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "tr";
            var simpleTrigger = Mapper.GetSimpleTrigger(xpandSimpleTrigger, "jb", typeof(DummyJob));

            simpleTrigger.StartTimeUtc = DateTime.UtcNow;
            
            Scheduler.ScheduleJob(simpleTrigger);
            Scheduler.Start();
        };

        Because of = () => Thread.Sleep(5000);

        It should_execute_the_job = () => JobExecutedCount.ShouldEqual(1);
    }

    public class MyClass : With_Scheduler {
        Establish context = () => {
            Scheduler.Start();
            var simpleTrigger = new SimpleTrigger("tr",typeof(DummyJob).FullName)
                                {JobName = JobDetail.Name,JobGroup = JobDetail.Group, RepeatCount = 2,RepeatInterval = TimeSpan.FromSeconds(.1)};
            simpleTrigger.StartTimeUtc = DateTime.UtcNow;
            Scheduler.ScheduleJob(simpleTrigger);
            
        };
        Because of = () => Thread.Sleep(5000);

        It should_execute_the_job_twice = () => JobExecutedCount.ShouldEqual(2);
    }
}
