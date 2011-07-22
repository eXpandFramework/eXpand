using DevExpress.ExpressApp;
using Quartz.Core;
using Quartz.Impl;

namespace Xpand.ExpressApp.JobScheduler.QuartzExtensions {
    public interface IXpandScheduler {
        XafApplication Application { get; }
    }

    public class XpandScheduler : StdScheduler, IXpandScheduler {
        readonly XafApplication _application;

        public XpandScheduler(QuartzScheduler sched, XafApplication application)
            : base(sched) {
            _application = application;
        }

        public XafApplication Application {
            get { return _application; }
        }
    }

}