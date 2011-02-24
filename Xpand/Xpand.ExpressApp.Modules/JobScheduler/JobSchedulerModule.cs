using System;
using DevExpress.ExpressApp;
using Quartz;
using Quartz.Core;
using Quartz.Impl;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    public sealed partial class JobSchedulerModule : XpandModuleBase {

        public JobSchedulerModule() {
            InitializeComponent();

        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory();
            _scheduler = (XpandScheduler) stdSchedulerFactory.GetScheduler();
        }
        XpandScheduler _scheduler;

        public XpandScheduler Scheduler {
            get { return _scheduler; }
        }
    }

    public class XpandSchedulerFactory : StdSchedulerFactory {
        protected override IScheduler Instantiate(QuartzSchedulerResources resources, QuartzScheduler quartzScheduler) {
            var schedulingContext = new SchedulingContext { InstanceId = resources.InstanceId };
            IScheduler stdScheduler = new XpandScheduler(quartzScheduler, schedulingContext, resources, schedulingContext);
            
            return stdScheduler;
        }
    }

    public class XpandScheduler:StdScheduler {
        readonly QuartzSchedulerResources _resources;
        readonly SchedulingContext _schedulingContext;

        public XpandScheduler(QuartzScheduler sched, SchedulingContext schedCtxt, QuartzSchedulerResources resources, SchedulingContext schedulingContext) : base(sched, schedCtxt) {
            _resources = resources;
            _schedulingContext = schedulingContext;
        }

        public SchedulingContext SchedulingContext {
            get { return _schedulingContext; }
        }

        public QuartzSchedulerResources Resources {
            get { return _resources; }
        }

        public JobDetail GetJobDetail(IJobDetail jobDetail) {
            return GetJobDetail(jobDetail.Name,jobDetail.Group);
        }

        public void TriggerJob(IJobDetail jobDetail) {
            TriggerJob(jobDetail.Name,jobDetail.Group);
        }
    }
}