using System.Collections.Specialized;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation;
using Quartz;
using Quartz.Impl.Matchers;
using Xpand.ExpressApp.JobScheduler.Qaurtz;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.JobScheduler {
    public sealed class JobSchedulerModule : XpandModuleBase {

        public JobSchedulerModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));

        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            var properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = Application.Title;
            properties["quartz.scheduler.instanceId"] = Assembly.GetAssembly(Application.GetType()).ManifestModule.Name;
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "5";
            properties["quartz.threadPool.threadPriority"] = "Normal";
            properties["quartz.jobStore.misfireThreshold"] = "60000";
            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            properties["quartz.jobStore.dataSource"] = "default";
            properties["quartz.jobStore.tablePrefix"] = "QRTZ_";
            properties["quartz.jobStore.clustered"] = "true";

            properties["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz";

            properties["quartz.dataSource.default.connectionString"] = "Server=(local);Database=quartz1;Trusted_Connection=True;";
            properties["quartz.dataSource.default.provider"] = "SqlServer-20";
            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory(properties);
            _scheduler = (IXpandScheduler)stdSchedulerFactory.GetScheduler();
            _scheduler.ListenerManager.AddJobListener(new XpandJobListener(), EverythingMatcher<JobKey>.AllJobs());
            _scheduler.ListenerManager.AddTriggerListener(new XpandTriggerListener(), EverythingMatcher<JobKey>.AllTriggers());
            ((XpandScheduler) _scheduler).Application = Application;
            _scheduler.Start();
        }
        IXpandScheduler _scheduler;

        public IXpandScheduler Scheduler {
            get { return _scheduler; }
        }

    }
}