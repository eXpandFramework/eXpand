using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Machine.Specifications;
using Quartz;
using Quartz.Spi;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.JobScheduler.Jobs.SendEmail;
using Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation;
using Xpand.Persistent.BaseImpl.JobScheduler;
using Xpand.ExpressApp.JobScheduler.Qaurtz;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_SendThresholdCalculationEmail_Job_is_executed {
        static JobExecutionContext _jobExecutionContext;
        static SendThresholdCalculationEmailJob _sendThresholdCalculationEmailJob;

        Establish context = () => {
            _sendThresholdCalculationEmailJob = new SendThresholdCalculationEmailJob();
            var objectSpace = ObjectSpaceInMemory.CreateNew();
            var xpandJobDetail = objectSpace.CreateObject<XpandJobDetail>();
            xpandJobDetail.Name = "s";
            var xpandJobDetailDataMap = objectSpace.CreateObject<SendEmailJobDetailDataMap>();
            xpandJobDetailDataMap.Emails = "apostolis.bekiaris@gmail.com";
            xpandJobDetail.JobDetailDataMap = xpandJobDetailDataMap;
            xpandJobDetail.Job = objectSpace.CreateObject<XpandJob>();
            xpandJobDetail.Job.JobType = typeof (SendThresholdCalculationEmailJob);
            var xpandJobDataMap = objectSpace.CreateObject<SendEmailJobDataMap>();
            xpandJobDetail.Job.JobDataMap = xpandJobDataMap;
            xpandJobDataMap.EmailTemplate = SchedulerResource.EmailTemplate;

            
            ISchedulerFactory stdSchedulerFactory = new XpandSchedulerFactory();
            var scheduler = (IXpandScheduler) stdSchedulerFactory.GetScheduler();
            Isolate.WhenCalled(() => scheduler.Application.ObjectSpaceProvider.TypesInfo).WillReturn(XafTypesInfo.Instance);
            var storeJob = scheduler.StoreJob(xpandJobDetail);
            storeJob.JobDataMap.Add(ThresholdCalculationJob.ThresholdCalcCount, 2);

            _jobExecutionContext = new JobExecutionContext(null, new TriggerFiredBundle(storeJob, Isolate.Fake.Instance<Trigger>(), null, false, null, null, null, null),
                                                              _sendThresholdCalculationEmailJob);
        };

        Because of = () => _sendThresholdCalculationEmailJob.Execute(_jobExecutionContext);
        It should_should = () => { };
    }
}
