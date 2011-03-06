using System.Collections.Specialized;
using Quartz;
using Quartz.Core;
using Quartz.Impl;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public class XpandSchedulerFactory : StdSchedulerFactory {
        public XpandSchedulerFactory(NameValueCollection props)
            : base(props) {
        }

        public XpandSchedulerFactory() {
        }

        protected override IScheduler Instantiate(QuartzSchedulerResources resources, QuartzScheduler quartzScheduler) {
            var schedulingContext = new SchedulingContext { InstanceId = resources.InstanceId };
            return new XpandScheduler(quartzScheduler, schedulingContext, resources, schedulingContext);
        }

    }
}