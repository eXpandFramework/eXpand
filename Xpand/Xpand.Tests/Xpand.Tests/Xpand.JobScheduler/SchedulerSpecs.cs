using System;
using System.Collections.Specialized;
using System.Threading;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Quartz;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.JobScheduler;
using Xpand.ExpressApp.JobScheduler.Qaurtz;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_trigger_time_pass {
        Establish context = () => {
            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory();
            var scheduler = (XpandScheduler) stdSchedulerFactory.GetScheduler();
            
            
            var dummyJob = new DummyJob();
            Isolate.Swap.NextInstance<DummyJob>().With(dummyJob);
            Isolate.WhenCalled(() => dummyJob.Execute(null)).DoInstead(callContext => _jobExecuted = true);
            var jobDetail = new JobDetail("jb", typeof(DummyJob).FullName, typeof(DummyJob));
            
            var objectSpace = ObjectSpaceInMemory.CreateNew();
            var xpandSimpleTrigger = objectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "tr";
            var simpleTrigger = Mapper.GetSimpleTrigger(xpandSimpleTrigger, "jb", typeof(DummyJob));

            simpleTrigger.StartTimeUtc = DateTime.UtcNow;
            scheduler.StoreJob(jobDetail);
            scheduler.ScheduleJob(simpleTrigger);

            scheduler.Start();
        };

        static bool _jobExecuted;
        Because of = () => Thread.Sleep(5000);

        It should_have_execute_associated_job = () => _jobExecuted.ShouldBeTrue();
    }

}
