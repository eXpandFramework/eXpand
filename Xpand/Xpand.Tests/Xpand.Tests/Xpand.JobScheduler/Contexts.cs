using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Quartz;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.JobScheduler;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class DummyJob : IJob {
        public void Execute(JobExecutionContext context) {

        }
    }

    public class With_Application<T> where T : With_Application<T> {
        protected static XpandJobDetail JobDetail;
        protected static IObjectSpace ObjectSpace;
        static T _instance;
        protected static IScheduler Scheduler;

        Establish context = () => {
            _instance = Activator.CreateInstance<T>();
            var jobSchedulerModule = new JobSchedulerModule();
            jobSchedulerModule.Setup(new ApplicationModulesManager());
            Scheduler = jobSchedulerModule.Scheduler;
            Isolate.Fake.XafApplicationInstance(typeof(XpandJobDetail), view => Instance.InitObject(view), window => window.Application.Modules.Add(jobSchedulerModule),
                                                Instance.GetControllers().ToArray());
        };

        protected virtual List<Controller> GetControllers() {
            return new List<Controller> { new CreateJobDetailController(), new JobSchedulerController() };
        }

        public static T Instance {
            get { return _instance; }
        }

        protected virtual void InitObject(DetailView detailView) {
            ObjectSpace = detailView.ObjectSpace;
            JobDetail = (XpandJobDetail)detailView.CurrentObject;
            JobDetail.JobType = typeof(DummyJob);
            JobDetail.Name = "name";
            JobDetail.Group = "group";
        }

    }
}
