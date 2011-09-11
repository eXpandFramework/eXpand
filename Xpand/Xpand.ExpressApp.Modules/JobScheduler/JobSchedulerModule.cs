using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.ExpressApp.SystemModule;
using System.Linq;

namespace Xpand.ExpressApp.JobScheduler {
    [ToolboxBitmap(typeof(JobSchedulerModule))]
    [ToolboxItem(true)]
    public sealed class JobSchedulerModule : XpandModuleBase {

        public JobSchedulerModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(AdditionalViewControlsProvider.AdditionalViewControlsModule));
            XafTypesInfo.Instance.LoadTypes(typeof(AnnualCalendar).Assembly);
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (Application == null)
                return;
            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory(Application);
            try {
                IScheduler scheduler = stdSchedulerFactory.AllSchedulers.SingleOrDefault();
                _scheduler = scheduler ?? stdSchedulerFactory.GetScheduler();
            } catch (Exception e) {
                if (!Debugger.IsAttached)
                    Tracing.Tracer.LogError(e);
            }
        }
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing && Scheduler is StdScheduler) {
                Scheduler.Shutdown();
            }
        }
        IScheduler _scheduler;
        public IScheduler Scheduler {
            get { return _scheduler; }
        }

    }
}