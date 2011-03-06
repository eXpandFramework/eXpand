using System.Collections.Specialized;
using DevExpress.ExpressApp;
using Quartz;
using Xpand.ExpressApp.JobScheduler.Qaurtz;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.JobScheduler {
    public sealed class JobSchedulerModule : XpandModuleBase {

        public JobSchedulerModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));

        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            var properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "TestScheduler";
            properties["quartz.scheduler.instanceId"] = "instance_one";
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "5";
            properties["quartz.threadPool.threadPriority"] = "Normal";
            properties["quartz.jobStore.misfireThreshold"] = "60000";
            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            properties["quartz.jobStore.useProperties"] = "false";
            properties["quartz.jobStore.dataSource"] = "default";
            properties["quartz.jobStore.tablePrefix"] = "QRTZ_";
            properties["quartz.jobStore.clustered"] = "true";

            properties["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz";

            properties["quartz.dataSource.default.connectionString"] = "Server=(local);Database=quartz1;Trusted_Connection=True;";
            properties["quartz.dataSource.default.provider"] = "SqlServer-20";
            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory(properties);
            _scheduler = (IXpandScheduler)stdSchedulerFactory.GetScheduler();

            _scheduler.Start();
        }
        IXpandScheduler _scheduler;

        public IXpandScheduler Scheduler {
            get { return _scheduler; }
        }
    }
}