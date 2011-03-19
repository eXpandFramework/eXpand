using DevExpress.ExpressApp;
using Quartz.Core;
using Quartz.Impl;

namespace Xpand.ExpressApp.JobScheduler.QuartzExtensions {
    public interface IXpandScheduler {
        XafApplication Application { get; }
    }

    public class XpandScheduler:StdScheduler,IXpandScheduler {
        readonly XafApplication _application;

        public XpandScheduler(QuartzScheduler sched,XafApplication application) : base(sched) {
            _application = application;
        }

        public XafApplication Application {
            get { return _application; }
        }
    }
//    public interface IXpandScheduler : IScheduler {
//        XafApplication Application { get; }
//        IJobDetail GetJobDetail(IXpandJobDetail jobDetail);
//        void TriggerJob(IXpandJobDetail jobDetail);
//        IList<ITrigger> GetTriggersOfJob(IXpandJobDetail jobDetail);
//        bool DeleteJob(IXpandJobDetail jobDetail);
//        bool UnscheduleJob(string triggerName, Type jobType, string jobName, string jobGroup);
//        bool UnscheduleJob(string triggerName, Type jobType, string jobName);
//        IJobDetail StoreJob(IXpandJobDetail xpandJobDetail);
//        void StoreJob(IJobDetail jobDetail);
//        void StoreTrigger(IOperableTrigger simpleTrigger);
//        JobDetailImpl GetJobDetail(string jobDetail, Type jobGroup);
//
//
//        DateTimeOffset? RescheduleJob(IOperableTrigger trigger);
//        bool HasTriggers(IXpandJobDetail jobDetail);
//        DateTimeOffset ScheduleJob(IXpandJobTrigger jobTrigger, IXpandJobDetail jobDetail, string groupName);
//        void StoreTrigger(IXpandJobTrigger jobTrigger, IXpandJobDetail jobDetail, string groupName);
//    }

//    public class XpandScheduler : RemoteScheduler, IXpandScheduler {
//        readonly IJobStore _jobStore;
//        public const string TriggerJobListenersOn = "TriggerJobListenersOn";
//        public const string TriggerTriggerJobListenersOn = "TriggerTriggerJobListenersOn";
//
//        public XpandScheduler(RemoteScheduler remoteScheduler, IJobStore jobStore)
//            : base(remoteScheduler.SchedulerInstanceId) {
//                remoteScheduler.r
//            _jobStore = jobStore;
//        }
//
//
//        public XafApplication Application { get; set; }
//
//
//        
//    }
}