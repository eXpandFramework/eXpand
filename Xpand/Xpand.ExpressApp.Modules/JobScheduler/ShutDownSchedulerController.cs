using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Core;

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
            var scheduler = Application.FindModule<JobSchedulerModule>().Scheduler;
            if (!scheduler.IsStarted && !scheduler.IsShutdown) {
                scheduler.Shutdown();
            }
        }
    }
}