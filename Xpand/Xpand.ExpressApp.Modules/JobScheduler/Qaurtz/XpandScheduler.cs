using Quartz;
using Quartz.Core;
using Quartz.Impl;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public class XpandScheduler : StdScheduler {
        readonly QuartzSchedulerResources _resources;
        readonly SchedulingContext _schedulingContext;

        public XpandScheduler(QuartzScheduler sched, SchedulingContext schedCtxt, QuartzSchedulerResources resources, SchedulingContext schedulingContext)
            : base(sched, schedCtxt) {
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
            return GetJobDetail(jobDetail.Name, jobDetail.Group);
        }

        public void TriggerJob(IJobDetail jobDetail) {
            TriggerJob(jobDetail.Name, jobDetail.Group);
        }
    }
}