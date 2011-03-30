using System;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation;
using Quartz;
using Quartz.Impl.Calendar;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.ExpressApp.SystemModule;
using System.Linq;

namespace Xpand.ExpressApp.JobScheduler {
    public sealed class JobSchedulerModule : XpandModuleBase {

        public JobSchedulerModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(AdditionalViewControlsProvider.AdditionalViewControlsModule));
            XafTypesInfo.Instance.LoadTypes(typeof(AnnualCalendar).Assembly);
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (Application==null)
                return;
            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory(Application);
            try {
                IScheduler scheduler = stdSchedulerFactory.AllSchedulers.SingleOrDefault();
                _scheduler = scheduler ?? stdSchedulerFactory.GetScheduler();
            }
            catch (Exception) {
                if (!Debugger.IsAttached)
                    throw;  
            }
        }
        IScheduler _scheduler;

        public IScheduler Scheduler {
            get { return _scheduler; }
        }

    }
}