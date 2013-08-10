using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Quartz;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.JobScheduler {
    public abstract class SupportSchedulerController : ViewController {

        public IScheduler Scheduler {
            get { return Application.FindModule<JobSchedulerModule>().Scheduler; }
        }

        public ITypesInfo TypesInfo {
            get { return Application.ObjectSpaceProvider.TypesInfo; }
        }

    }
}
