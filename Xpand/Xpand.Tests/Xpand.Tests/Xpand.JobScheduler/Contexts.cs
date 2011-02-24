using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Quartz;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.JobScheduler;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    [JobType(typeof(DummyJob))]
    public class DummyJobListener:IJobListener {
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
    public abstract class With_Application<T> where T : With_Application<T> {
        protected static XpandJobDetail JobDetail;
        protected static IObjectSpace ObjectSpace;
        protected static DetailView DetailView;
        protected static XafApplication Application;
        static T _instance;
        protected static XpandScheduler Scheduler;

        internal Establish Context = () => {
            _instance = Activator.CreateInstance<T>();
            XafTypesInfo.Instance.FindTypeInfo(typeof (DummyJobListener));
            Application = Isolate.Fake.XafApplicationInstance(typeof(XpandJobDetail), Instance.ViewCreated, Instance.WindowCreated,
                                                                             Instance.GetControllers().ToArray());
        };

        protected virtual void WindowCreated(Window window) {
            var jobSchedulerModule = new JobSchedulerModule();
            jobSchedulerModule.Setup(new ApplicationModulesManager());
            Scheduler = jobSchedulerModule.Scheduler;
            window.Application.Modules.Add(jobSchedulerModule);
        }
        protected virtual List<Controller> GetControllers() {
            return new List<Controller> { new CreateJobDetailController(), new JobSchedulerController() };
        }

        public static T Instance {
            get { return _instance; }
        }

        protected virtual void ViewCreated(DetailView detailView) {
            DetailView = detailView;
            ObjectSpace = detailView.ObjectSpace;
            JobDetail = (XpandJobDetail)detailView.CurrentObject;
            JobDetail.JobType = typeof(DummyJob);
            JobDetail.Name = "name";
            JobDetail.Group = "group";
        }

    }
}
