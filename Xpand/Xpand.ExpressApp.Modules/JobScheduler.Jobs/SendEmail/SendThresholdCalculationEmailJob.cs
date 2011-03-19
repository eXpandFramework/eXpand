using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using DevExpress.ExpressApp.Utils;
using Quartz;
using Xpand.EmailTemplateEngine;
using Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.JobScheduler.Jobs.SendEmail {
    [JobDetailDataMapType(typeof(SendEmailJobDetailDataMap))]
    [JobDataMapType(typeof(SendEmailJobDataMap))]
    public class SendThresholdCalculationEmailJob : IJob {
        public void Execute(IJobExecutionContext context) {
            var jobDataMap = context.MergedJobDataMap;
            var count = jobDataMap.GetInt<ThresholdJobDetailDataMap>(ThresholdCalculationJob.ThresholdCalcCount);
            if (count > 0) {
                SendEmail(jobDataMap, count);
            }
        }

        void SendEmail(JobDataMap jobDataMap, int count) {

            IEmailTemplateContentReader templateReader = new StreamEmailTemplateContentReader(jobDataMap.GetString<SendEmailJobDataMap>("EmailTemplate"));
            IEmailTemplateEngine templateEngine = new EmailTemplateEngine.EmailTemplateEngine(templateReader);
            IEmailSender sender = new EmailSender {
                CreateClientFactory = () => new SmtpClientWrapper(CreateSmtpClient())
            };
            var subsystem = new ThresholdCalculationEmailSubsystem(templateEngine, sender, jobDataMap.GetString<SendEmailJobDataMap>("Name"));
            var thresholdSeverity = (ThresholdSeverity)jobDataMap.Get<ThresholdJobDetailDataMap>("Severity");
            subsystem.SendThresholdCalculationMail(jobDataMap.GetString<SendEmailJobDataMap>("SubjectTemplate"),
                                                   jobDataMap.GetString<ThresholdJobDetailDataMap>("Criteria"), count,
                                                   (Type)jobDataMap.Get<ThresholdJobDetailDataMap>("DataType"),
                                                   thresholdSeverity,
                                                   jobDataMap.GetString<SendEmailJobDetailDataMap>("Emails"));
        }
        private SmtpClient CreateSmtpClient() {
            var smtpClient = new SmtpClient {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = ConfigurationManager.AppSettings["ThresholdEmailJobHost"].Split(':')[0],
                Port = ConfigurationManager.AppSettings["ThresholdEmailJobHost"].Split(':').Count() > 0 ? (Convert.ToInt32(ConfigurationManager.AppSettings["ThresholdEmailJobHost"].Split(':')[1])) : 25,
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
        readonly string _name;

        public ThresholdCalculationEmailSubsystem(IEmailTemplateEngine templateEngine, IEmailSender sender, string name) {
            _name = name;
            TemplateEngine = templateEngine;
            Sender = sender;
        }

        protected IEmailTemplateEngine TemplateEngine { get; private set; }

        protected IEmailSender Sender { get; private set; }



        public virtual void SendThresholdCalculationMail(string subjectTemplate, string criteria, int count, Type type, ThresholdSeverity severity, string to) {
            var model = new {
                Criteria = criteria,
                Count = count,
                DataType = CaptionHelper.GetClassCaption(type.FullName),
                Severity = severity
            };
            var mail = TemplateEngine.Execute(_name, model);
            to.Split(';').Each(s => mail.To.Add(s));
            mail.From = ConfigurationManager.AppSettings["ThresholdEmailJobFrom"];
            mail.Subject = subjectTemplate;
            Sender.Send(mail);
        }
    }

}