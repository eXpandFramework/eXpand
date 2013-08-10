using System;
using DevExpress.ExpressApp;
using Quartz.Impl;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.JobScheduler {
    public class ShutDownSchedulerController : WindowController {
        public ShutDownSchedulerController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated() {
            base.OnActivated();
            Window.Disposing += WindowOnDisposing;
        }

        void WindowOnDisposing(object sender, EventArgs eventArgs) {
            var jobSchedulerModule = Application.FindModule<JobSchedulerModule>();
            var scheduler = jobSchedulerModule.Scheduler;
            if (scheduler is StdScheduler) {
                scheduler.Shutdown();
            }
        }
    }
}