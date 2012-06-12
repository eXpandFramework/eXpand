using System;
using System.Collections.Specialized;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Machine.Specifications;
using Quartz;
using Quartz.Impl;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.JobScheduler;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class With_Scheduler {
        protected static IJobDetail JobDetail;
        protected static int JobExecutedCount;
        protected static IScheduler Scheduler;

        Establish context = () => {
            new JobSchedulerModule();
            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory(SchedulerConfig.GetProperties(), Isolate.Fake.Instance<XafApplication>());
            Scheduler = stdSchedulerFactory.GetScheduler();
            JobDetail = new JobDetailImpl("jb", typeof(DummyJob).FullName, typeof(DummyJob));
            Scheduler.StoreJob(JobDetail);

            var dummyJob = new DummyJob();
            Isolate.Swap.NextInstance<DummyJob>().With(dummyJob);
            Isolate.WhenCalled(() => dummyJob.Execute(null)).DoInstead(callContext => JobExecutedCount++);
        };
    }


    public class DummyJobListener : IJobListener {
        public void JobToBeExecuted(IJobExecutionContext context) {
            throw new NotImplementedException();
        }

        public void JobExecutionVetoed(IJobExecutionContext context) {
            throw new NotImplementedException();
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException) {
            throw new NotImplementedException();
        }

        public string Name {
            get { return GetType().Name; }
        }
    }

    public class DummyDataMapObject : BaseObject {
        public DummyDataMapObject(Session session)
            : base(session) {
        }
    }
    [JobDetailDataMapType(typeof(DummyDataMapObject))]
    public class DummyJob : IJob {
        public void Execute(IJobExecutionContext context) {

        }
    }
    public class DummyJob2 : IJob {
        public void Execute(IJobExecutionContext context) {

        }
    }

    public class DummyDetailDataMap : XpandJobDetailDataMap {
        public DummyDetailDataMap(Session session)
            : base(session) {
        }
    }


    public abstract class With_Job_Scheduler_XpandJobDetail_Application<T> : With_Job_Scheduler_Application<T, XpandJobDetail> where T : With_Job_Scheduler_XpandJobDetail_Application<T> {
        protected override void ViewCreated(DetailView detailView) {
            base.ViewCreated(detailView);
            XPObjectSpace = detailView.ObjectSpace;
            Object.Job = XPObjectSpace.CreateObject<XpandJob>();
            Object.Job.JobType = typeof(DummyJob);
            Object.JobDetailDataMap = XPObjectSpace.CreateObject<DummyDetailDataMap>();
            Object.Name = "name";
        }

    }
    public abstract class With_Job_Scheduler_Application<T, TObject> : With_Application<T, TObject> where T : With_Job_Scheduler_Application<T, TObject> {
        protected static IScheduler Scheduler;
        JobSchedulerModule _jobSchedulerModule;
        protected override void WindowCreated(Window window) {
            base.WindowCreated(window);
            window.Application.Modules.Add(_jobSchedulerModule);
        }
        protected override System.Collections.Generic.List<Controller> GetControllers() {
            var controllers = base.GetControllers();
            controllers.Add(new JobDetailController());
            controllers.Add(new JobTriggerLinkController());
            controllers.Add(new JobTriggerController());
            return controllers;
        }
        protected override void Initialize() {
            base.Initialize();
            _jobSchedulerModule = new JobSchedulerModule();
            Isolate.Swap.AllInstances<JobSchedulerModule>().With(_jobSchedulerModule);
            var properties = SchedulerConfig.GetProperties();
            IScheduler scheduler = new XpandSchedulerFactory(properties, Application).GetScheduler();
            Isolate.WhenCalled(() => _jobSchedulerModule.Scheduler).WillReturn(scheduler);
            Scheduler = scheduler;
            XafTypesInfo.Instance.FindTypeInfo(typeof(DummyJobListener));
        }

    }

    internal class SchedulerConfig {
        public static NameValueCollection GetProperties() {
            var properties = new NameValueCollection();
            properties["quartz.checkConfiguration"] = "False";
            properties["quartz.scheduler.instanceName"] = "PriorityExampleScheduler";
            // Set thread count to 1 to force Triggers scheduled for the same time to 
            // to be ordered by priority.
            properties["quartz.threadPool.threadCount"] = "1";
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz";
            return properties;
        }

    }

}
