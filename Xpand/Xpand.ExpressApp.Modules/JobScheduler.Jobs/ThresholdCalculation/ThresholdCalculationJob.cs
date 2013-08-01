using System;
using Common.Logging;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Quartz;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Xpo;

namespace Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation {
    [System.ComponentModel.DisplayName("Threshold")]
    [JobDetailDataMapType(typeof(ThresholdJobDetailDataMap))]
    public class ThresholdCalculationJob : IJob {
        public const string ThresholdCalcCount = "ThresholdCalcCount";

        readonly ILog log = LogManager.GetLogger(typeof(ThresholdCalculationJob));
        public void Execute(IJobExecutionContext context) {
            log.Info("EXECUTING:ThresholdCalculationJob");
            var application = ((IXpandScheduler)context.Scheduler).Application;
            IObjectSpaceProvider objectSpaceProvider = application.ObjectSpaceProvider;
            var jobDataMap = context.JobDetail.JobDataMap;
            var typeInfo = objectSpaceProvider.TypesInfo.FindTypeInfo((Type)jobDataMap.Get<ThresholdJobDetailDataMap>(map => map.DataType));
            object count;
            using (var unitOfWork = new UnitOfWork(((XpandObjectSpaceProvider)objectSpaceProvider).WorkingDataLayer)) {
                count = unitOfWork.GetCount(typeInfo.Type, CriteriaOperator.Parse(jobDataMap.GetString<ThresholdJobDetailDataMap>(map => map.Criteria)));
            }
            jobDataMap.Put<ThresholdJobDetailDataMap>(ThresholdCalcCount, count);
        }
    }

}
