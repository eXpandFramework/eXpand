
using System.Data;
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
    public class When_new_Job_detail_saved {
        static XpandJobDetail _jobDetail;
        static IScheduler _scheduler;
        static IObjectSpace _objectSpace;

        Establish context = () => {

            var dataSet = new DataSet();
            _objectSpace = ObjectSpaceInMemory.CreateNew(dataSet);

            var jobSchedulerModule = new JobSchedulerModule();
            jobSchedulerModule.Setup(new ApplicationModulesManager());
            _scheduler = jobSchedulerModule.Scheduler;
            Isolate.Fake.XafApplicationInstance(typeof (XpandJobDetail), InitObject,window => window.Application.Modules.Add(jobSchedulerModule),
                                                                                        new Controller[] {new JobDetailController()});
            
        };

        static void InitObject(DetailView detailView) {
            _objectSpace = detailView.ObjectSpace;
            _jobDetail = (XpandJobDetail) detailView.CurrentObject;
            _jobDetail.JobType = typeof(DummyJob);
            _jobDetail.Name = "name";
            _jobDetail.Group = "group";
        }


        Because of = () => _objectSpace.CommitChanges();

        It should_add_a_new_job_detail_to_the_scheduler =
            () => _scheduler.GetJobDetail(_jobDetail.Name, _jobDetail.Group).ShouldNotBeNull();
    }

    public class When_Job_detail_Deleted {
        static IScheduler _scheduler;
        static IObjectSpace _objectSpace;

        static XpandJobDetail _jobDetail;

        Establish context = () => {
            var dataSet = new DataSet();
            _objectSpace = ObjectSpaceInMemory.CreateNew(dataSet);

            var jobSchedulerModule = new JobSchedulerModule();
            jobSchedulerModule.Setup(new ApplicationModulesManager());
            _scheduler = jobSchedulerModule.Scheduler;
            Isolate.Fake.XafApplicationInstance(typeof(XpandJobDetail), InitObject, window => window.Application.Modules.Add(jobSchedulerModule),
                                                new Controller[] { new JobDetailController() });
            _objectSpace.CommitChanges();
        };

        Because of = () => _objectSpace.Delete(_jobDetail);

        static void InitObject(DetailView detailView) {
            _objectSpace = detailView.ObjectSpace;
            _jobDetail = (XpandJobDetail)detailView.CurrentObject;
            _jobDetail.JobType = typeof(DummyJob);
            _jobDetail.Name = "name";
            _jobDetail.Group = "group";
        }

        It should_remove_it_from_the_scheduler =
            () => _scheduler.GetJobDetail(_jobDetail.Name, _jobDetail.Group).ShouldBeNull();
    }
    public class When_Job_detail_updated {
        static IScheduler _scheduler;
        static IObjectSpace _objectSpace;

        static XpandJobDetail _jobDetail;

        Establish context = () => {
            var dataSet = new DataSet();
            _objectSpace = ObjectSpaceInMemory.CreateNew(dataSet);

            var jobSchedulerModule = new JobSchedulerModule();
            jobSchedulerModule.Setup(new ApplicationModulesManager());
            _scheduler = jobSchedulerModule.Scheduler;
            Isolate.Fake.XafApplicationInstance(typeof(XpandJobDetail), InitObject, window => window.Application.Modules.Add(jobSchedulerModule),
                                                new Controller[] { new JobDetailController() });
            _objectSpace.CommitChanges();
            _jobDetail.Group = "changed_group";
        };

        Because of = () => _objectSpace.CommitChanges();

        static void InitObject(DetailView detailView) {
            _objectSpace = detailView.ObjectSpace;
            _jobDetail = (XpandJobDetail)detailView.CurrentObject;
            _jobDetail.JobType = typeof(DummyJob);
            _jobDetail.Name = "name";
            _jobDetail.Group = "group";
        }

        It should_change_the_value_in_the_scheduler =
            () => _scheduler.GetJobDetail(_jobDetail.Name, _jobDetail.Group).Group.ShouldEqual("changed_group");
    }
}
