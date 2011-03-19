using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation;
using Quartz;
using Quartz.Impl.Calendar;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.JobScheduler {
    public sealed class JobSchedulerModule : XpandModuleBase {

        public JobSchedulerModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
            XafTypesInfo.Instance.LoadTypes(typeof(AnnualCalendar).Assembly);
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
//            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory(Application);
//            _scheduler = stdSchedulerFactory.GetScheduler();
        }
        IScheduler _scheduler;

        public IScheduler Scheduler {
            get { return _scheduler; }
        }

    }
}