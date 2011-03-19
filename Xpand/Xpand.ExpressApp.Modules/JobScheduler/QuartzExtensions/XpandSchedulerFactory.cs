using System.Collections.Specialized;
using DevExpress.ExpressApp;
using Quartz;
using Quartz.Core;
using Quartz.Impl;

namespace Xpand.ExpressApp.JobScheduler.QuartzExtensions {
    public class XpandSchedulerFactory : StdSchedulerFactory {
        readonly XafApplication _application;

        public XpandSchedulerFactory(NameValueCollection props,XafApplication application)
            : base(props) {
            _application = application;
        }

        public XpandSchedulerFactory(XafApplication application) {
            _application = application;
        }

        protected override IScheduler Instantiate(QuartzSchedulerResources resources, QuartzScheduler quartzScheduler) {
            var scheduler = base.Instantiate(resources, quartzScheduler);
            return scheduler is StdScheduler ? new XpandScheduler(quartzScheduler, _application) : scheduler;
        }
    }
}