using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.JobScheduler.Qaurtz;

namespace Xpand.ExpressApp.JobScheduler {
    public abstract class SupportSchedulerController : ViewController {
        
        public IXpandScheduler Scheduler {
            get { return Application.FindModule<JobSchedulerModule>().Scheduler; }
        }

        public ITypesInfo TypesInfo {
            get { return Application.ObjectSpaceProvider.TypesInfo; }
        }

    }
}
