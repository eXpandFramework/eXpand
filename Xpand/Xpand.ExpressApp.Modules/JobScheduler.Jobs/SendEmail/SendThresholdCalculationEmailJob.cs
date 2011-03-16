using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using DevExpress.ExpressApp.Utils;
using Quartz;
using Xpand.EmailTemplateEngine;
using Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Utils.Helpers;
using System.Linq;

namespace Xpand.ExpressApp.JobScheduler.Jobs.SendEmail {
    [JobDetailDataMapType(typeof(SendEmailJobDetailDataMap))]
    [JobDataMapType(typeof(SendEmailJobDataMap))]
    public class SendThresholdCalculationEmailJob : IJob {
        public void Execute(JobExecutionContext context) {
            var jobDataMap = context.MergedJobDataMap;
            var count = jobDataMap.GetInt(ThresholdCalculationJob.ThresholdCalcCount);
            if (count > 0) {
                SendEmail(jobDataMap,count);
            }
        }

        void SendEmail(JobDataMap jobDataMap, int count) {

            IEmailTemplateContentReader templateReader = new StreamEmailTemplateContentReader(jobDataMap.GetString("EmailTemplate"));
            IEmailTemplateEngine templateEngine = new EmailTemplateEngine.EmailTemplateEngine(templateReader);
            IEmailSender sender = new EmailSender {
                CreateClientFactory = () => new SmtpClientWrapper(CreateSmtpClient())
            };
            var subsystem = new ThresholdCalculationEmailSubsystem(templateEngine, sender);
            var thresholdSeverity = (ThresholdSeverity) jobDataMap.Get("Severity");
            subsystem.SendThresholdCalculationMail(jobDataMap.GetString("SubjectTemplate"),jobDataMap.GetString("Criteria"), count, (Type)jobDataMap.Get("DataType"), thresholdSeverity,jobDataMap.GetString("Emails"));
        }
        private SmtpClient CreateSmtpClient() {
            var smtpClient = new SmtpClient {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = ConfigurationManager.AppSettings["ThresholdEmailJobHost"].Split(':')[0],
                Port = ConfigurationManager.AppSettings["ThresholdEmailJobHost"].Split(':').Count()>0?(Convert.ToInt32(ConfigurationManager.AppSettings["ThresholdEmailJobHost"].Split(':')[1])):25,
                EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["ThresholdEmailJobEnableSsl"]),
                UseDefaultCredentials = true
            };
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ThresholdEmailJobFromPass"])) {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["ThresholdEmailJobFrom"],
                    ConfigurationManager.AppSettings["ThresholdEmailJobFromPass"]);
            }
            return smtpClient;
        }
    }
    public class ThresholdCalculationEmailSubsystem {
        public const string ThresholdCalculationEmail = "ThresholdCalculationEmail";

        public ThresholdCalculationEmailSubsystem(IEmailTemplateEngine templateEngine, IEmailSender sender) {
            TemplateEngine = templateEngine;
            Sender = sender;
        }

        protected IEmailTemplateEngine TemplateEngine { get; private set; }

        protected IEmailSender Sender { get; private set; }

        

        public virtual void SendThresholdCalculationMail(string subjectTemplate, string criteria, int count, Type type, ThresholdSeverity severity, string to) {
            var model = new {
                Criteria = criteria,
                Count = count,
                Type = CaptionHelper.GetClassCaption(type.FullName),
                Severity=severity
            };
            var mail = TemplateEngine.Execute(ThresholdCalculationEmail, model);
            to.Split(';').Each(s => mail.To.Add(s));
            mail.From = ConfigurationManager.AppSettings["ThresholdEmailJobFrom"];
            mail.Subject = subjectTemplate;
            Sender.Send(mail);
        }
    }

}