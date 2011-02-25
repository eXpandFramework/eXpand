using System;
using System.Threading;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Quartz;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.JobScheduler;
using Xpand.ExpressApp.JobScheduler.Qaurtz;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    [JobType(typeof(DummyJob))]
    public class DummyJobListener : IJobListener {
        public void JobToBeExecuted(JobExecutionContext context) {
            throw new NotImplementedException();
        }

        public void JobExecutionVetoed(JobExecutionContext context) {
            throw new NotImplementedException();
        }

        public void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException) {
            throw new NotImplementedException();
        }

        public string Name {
            get { return GetType().Name; }
        }
    }
    public class DummyJob : IJob {
        public void Execute(JobExecutionContext context) {

        }
    }
    public class DummyJob2 : IJob {
        public void Execute(JobExecutionContext context) {

        }
    }



    public abstract class With_Job_Scheduler_XpandJobDetail_Application<T> : With_Job_Scheduler_Application<T, XpandJobDetail> where T : With_Job_Scheduler_XpandJobDetail_Application<T> {
        protected override void ViewCreated(DetailView detailView) {
            base.ViewCreated(detailView);
            ObjectSpace = detailView.ObjectSpace;
            Object.JobType = typeof(DummyJob);
            Object.Name = "name";
        }

    }
    public abstract class With_Job_Scheduler_Application<T, TObject> : With_Application<T, TObject> where T : With_Job_Scheduler_Application<T, TObject> {
        protected static XpandScheduler Scheduler;
        protected override void WindowCreated(Window window) {
            base.WindowCreated(window);
            var jobSchedulerModule = new JobSchedulerModule();
            var scheduler = new XpandSchedulerFactory().GetScheduler();
            Isolate.WhenCalled(() => jobSchedulerModule.Scheduler).WillReturn((XpandScheduler) scheduler);
//            jobSchedulerModule.Setup(new ApplicationModulesManager());
            Scheduler = (XpandScheduler) scheduler;
            window.Application.Modules.Add(jobSchedulerModule);
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
            XafTypesInfo.Instance.FindTypeInfo(typeof(DummyJobListener));
        }
    }
}
