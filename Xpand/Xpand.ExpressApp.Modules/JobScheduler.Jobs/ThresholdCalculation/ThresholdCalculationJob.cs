using System;
using Common.Logging;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Quartz;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Xpo;

namespace Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation {
    [System.ComponentModel.DisplayName("ThresholdJob")]
    [JobDetailDataMapType(typeof(ThresholdJobDetailDataMap))]
    public class ThresholdCalculationJob : IJob {
        public const string ThresholdCalcCount = "ThresholdCalcCount";
        public const string DataType = "DataType";
        public const string Criteria = "Criteria";
        readonly ILog log = LogManager.GetLogger(typeof(ThresholdCalculationJob));
        public void Execute(IJobExecutionContext context) {
            log.Info("EXECUTING:ThresholdCalculationJob");
            var application = ((IXpandScheduler)context.Scheduler).Application;
            var objectSpaceProvider = application.ObjectSpaceProvider;
            var jobDataMap = context.JobDetail.JobDataMap;
            var typeInfo = objectSpaceProvider.TypesInfo.FindTypeInfo((Type)jobDataMap.Get<ThresholdJobDetailDataMap>(DataType));
            object count;
            using (var unitOfWork = new UnitOfWork(((ObjectSpaceProvider)objectSpaceProvider).WorkingDataLayer)) {
                count = unitOfWork.GetCount(typeInfo.Type, CriteriaOperator.Parse(jobDataMap.GetString<ThresholdJobDetailDataMap>(Criteria)));
            }
            log.Info("EXECUTING:ThresholdCalculationJob:"+count);
            jobDataMap.Put<ThresholdJobDetailDataMap>(ThresholdCalcCount, count);
        }
    }

}
