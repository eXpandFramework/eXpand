using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.JobScheduler.Qaurtz;

namespace Xpand.ExpressApp.JobScheduler {
    public abstract class SupportSchedulerController : ViewController {
        Mapper _mapper;

        protected override void OnActivated() {
            base.OnActivated();
            _mapper = new Mapper();
        }

        public Mapper Mapper {
            get { return _mapper; }
        }

        public IXpandScheduler Scheduler {
            get { return Application.FindModule<JobSchedulerModule>().Scheduler; }
        }

        public ITypesInfo TypesInfo {
            get { return Application.ObjectSpaceProvider.TypesInfo; }
        }

    }
}
