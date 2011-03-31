using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using DevExpress.ExpressApp.Utils;
using Quartz;
using Xpand.EmailTemplateEngine;
using Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation;
using Xpand.Utils.Helpers;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using System.Linq;

namespace Xpand.ExpressApp.JobScheduler.Jobs.SendEmail {
    public class ThresholdCalculationEmailSubsystem {
        readonly string _name;
        readonly JobDataMap _jobDataMap;

        public ThresholdCalculationEmailSubsystem(JobDataMap jobDataMap)
            : this(GetEmailTemplateEngine(jobDataMap), GetEmailSender(), jobDataMap.GetString<SendEmailJobDataMap>(dataMap => dataMap.Name)) {
            _jobDataMap = jobDataMap;
        }

        static EmailSender GetEmailSender() {
            return new EmailSender {
                                       CreateClientFactory = () => new SmtpClientWrapper(CreateSmtpClient())
                                   };
        }

        static EmailTemplateEngine.EmailTemplateEngine GetEmailTemplateEngine(JobDataMap jobDataMap) {
            return new EmailTemplateEngine.EmailTemplateEngine(new StreamEmailTemplateContentReader(jobDataMap.GetString<SendEmailJobDataMap>(map => map.EmailTemplate)));
        }

        ThresholdCalculationEmailSubsystem(IEmailTemplateEngine templateEngine, IEmailSender sender, string name) {
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
        private static SmtpClient CreateSmtpClient() {
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

        public void SendEmail(int count) {
            var thresholdSeverity = (ThresholdSeverity)_jobDataMap.Get<ThresholdJobDetailDataMap>(detailDataMap => detailDataMap.Severity);
            SendThresholdCalculationMail(_jobDataMap.GetString<SendEmailJobDataMap>(emailJobDataMap => emailJobDataMap.SubjectTemplate),
                                                   _jobDataMap.GetString<ThresholdJobDetailDataMap>(jobDetailDataMap => jobDetailDataMap.Criteria), count,
                                                   (Type)_jobDataMap.Get<ThresholdJobDetailDataMap>(thresholdJobDetailDataMap => thresholdJobDetailDataMap.DataType),
                                                   thresholdSeverity,
                                                   _jobDataMap.GetString<SendEmailJobDetailDataMap>(emailJobDetailDataMap => emailJobDetailDataMap.Emails));
        }
    }
}