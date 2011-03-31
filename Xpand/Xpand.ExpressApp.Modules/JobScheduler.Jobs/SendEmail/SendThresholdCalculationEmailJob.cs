using Quartz;
using Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler.Jobs.SendEmail {
    [JobDetailDataMapType(typeof(SendEmailJobDetailDataMap))]
    [JobDataMapType(typeof(SendEmailJobDataMap))]
    [System.ComponentModel.DisplayName("SendThresholdCalculationEmail")]
    public class SendThresholdCalculationEmailJob : IJob {
        public void Execute(IJobExecutionContext context) {
            var jobDataMap = context.MergedJobDataMap;
            var count = jobDataMap.GetInt<ThresholdJobDetailDataMap>(ThresholdCalculationJob.ThresholdCalcCount);
            if (count > 0) {
                var subsystem = new ThresholdCalculationEmailSubsystem(jobDataMap);
                subsystem.SendEmail(count);
            }
        }
    }
}