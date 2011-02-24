using DevExpress.ExpressApp;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.JobScheduler {
    public abstract class SupportSchedulerController:ViewController {
        public XpandScheduler Scheduler {
            get { return Application.FindModule<JobSchedulerModule>().Scheduler; }
        }

    }
}
