using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using DevExpress.ExpressApp.Utils;
using Quartz;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.JobScheduler.Jobs.SendEmail {
    public class ThresholdCalculationEmailSubsystem {
        readonly string _name;
        readonly JobDataMap _jobDataMap;
        private static readonly IRazorEngineService _razorEngineService;

        static ThresholdCalculationEmailSubsystem(){
            var configuration = new TemplateServiceConfiguration {
                Debug = Debugger.IsAttached,
                DisableTempFileLocking = !Debugger.IsAttached
            };
            _razorEngineService = RazorEngineService.Create(configuration);
        }
        public ThresholdCalculationEmailSubsystem(JobDataMap jobDataMap)
            : this( GetEmailSender(), jobDataMap.GetString<SendEmailJobDataMap>(dataMap => dataMap.Name)) {
            _jobDataMap = jobDataMap;
        }

        static SmtpClient GetEmailSender() {
            return CreateSmtpClient();
        }

        ThresholdCalculationEmailSubsystem(SmtpClient sender, string name) {
            _name = name;
            Sender = sender;
        }

        protected SmtpClient Sender { get; private set; }

        public virtual void SendThresholdCalculationMail(string subjectTemplate, string criteria, int count, Type type, ThresholdSeverity severity, string to) {
            var model = new {
                                Criteria = criteria,
                                Count = count,
                                DataType = CaptionHelper.GetClassCaption(type.FullName),
                                Severity = severity
                            };
            MailMessage mail=new MailMessage();
            to.Split(';').Each(s => mail.To.Add(s));
            var emailTemplate = _jobDataMap.GetString<SendEmailJobDataMap>(map => map.EmailTemplate);
            mail.Body = _razorEngineService.RunCompile(emailTemplate, Guid.NewGuid().ToString(), null, model);
            mail.From = new MailAddress(ConfigurationManager.AppSettings["ThresholdEmailJobFrom"]);
            mail.Subject = subjectTemplate;
            Sender.Send(mail);
        }

        private static SmtpClient CreateSmtpClient() {
            var smtpClient = new SmtpClient {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = ConfigurationManager.AppSettings["ThresholdEmailJobHost"].Split(':')[0],
                Port = ConfigurationManager.AppSettings["ThresholdEmailJobHost"].Split(':').Any() ? (Convert.ToInt32(ConfigurationManager.AppSettings["ThresholdEmailJobHost"].Split(':')[1])) : 25,
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