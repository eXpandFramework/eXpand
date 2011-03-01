using System.Collections.Specialized;
using DevExpress.ExpressApp.DC;
using Quartz;
using Quartz.Core;
using Quartz.Impl;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public class XpandSchedulerFactory : StdSchedulerFactory {
        readonly ITypesInfo _typesInfo;

        public XpandSchedulerFactory(NameValueCollection props, ITypesInfo typesInfo)
            : base(props) {
            _typesInfo = typesInfo;
        }

        public XpandSchedulerFactory(ITypesInfo typesInfo) {
            _typesInfo = typesInfo;
        }

        protected override IScheduler Instantiate(QuartzSchedulerResources resources, QuartzScheduler quartzScheduler) {
            var schedulingContext = new SchedulingContext { InstanceId = resources.InstanceId };
            return new XpandScheduler(quartzScheduler, schedulingContext, resources, schedulingContext);
        }

    }
}