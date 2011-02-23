using DevExpress.ExpressApp;
using Quartz;
using Quartz.Impl;

namespace Xpand.ExpressApp.JobScheduler {
    public sealed partial class JobSchedulerModule : XpandModuleBase {

        public JobSchedulerModule() {
            InitializeComponent();

        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            ISchedulerFactory stdSchedulerFactory = new StdSchedulerFactory();
            _scheduler = stdSchedulerFactory.GetScheduler();
        }
        IScheduler _scheduler;

        public IScheduler Scheduler {
            get { return _scheduler; }
        }
    }
}