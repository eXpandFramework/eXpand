using DevExpress.ExpressApp;
using Machine.Specifications;
using Quartz;
using Quartz.Spi;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.JobScheduler.Jobs.SendEmail;
using Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_SendThresholdCalculationEmail_Job_is_executed {
        static IJobExecutionContext _jobExecutionContext;
        static SendThresholdCalculationEmailJob _sendThresholdCalculationEmailJob;

        Establish context = () => {
            _sendThresholdCalculationEmailJob = new SendThresholdCalculationEmailJob();
            var XPObjectSpace = ObjectSpaceInMemory.CreateNew();
            var xpandJobDetail = XPObjectSpace.CreateObject<XpandJobDetail>();
            xpandJobDetail.Name = "s";
            var xpandJobDetailDataMap = XPObjectSpace.CreateObject<SendEmailJobDetailDataMap>();
            xpandJobDetailDataMap.Emails = "apostolis.bekiaris@gmail.com";
            xpandJobDetail.JobDetailDataMap = xpandJobDetailDataMap;
            xpandJobDetail.Job = XPObjectSpace.CreateObject<XpandJob>();
            xpandJobDetail.Job.JobType = typeof(SendThresholdCalculationEmailJob);
            var xpandJobDataMap = XPObjectSpace.CreateObject<SendEmailJobDataMap>();
            xpandJobDetail.Job.JobDataMap = xpandJobDataMap;
            xpandJobDataMap.EmailTemplate = SchedulerResource.EmailTemplate;


            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory(SchedulerConfig.GetProperties(), Isolate.Fake.Instance<XafApplication>());
            var scheduler = stdSchedulerFactory.GetScheduler();
            var storeJob = scheduler.StoreJob(xpandJobDetail, XafTypesInfo.Instance);
            storeJob.JobDataMap.Add(ThresholdCalculationJob.ThresholdCalcCount, 2);

            _jobExecutionContext = new JobExecutionContextImpl(null, new TriggerFiredBundle(storeJob, Isolate.Fake.Instance<IOperableTrigger>(), null, false, null, null, null, null),
                                                              _sendThresholdCalculationEmailJob);
        };

        Because of = () => _sendThresholdCalculationEmailJob.Execute(_jobExecutionContext);
        It should_should = () => { };
    }
}
