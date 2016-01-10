using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using RazorEngine.Templating;
using Xpand.ExpressApp.Email.BusinessObjects;
using Xpand.ExpressApp.Email.Model;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Email.Logic {
    public class EmailRuleViewController : ViewController {
        public const string RuleObjectKeyValue = "RuleObjectKeyValue";
        LogicRuleViewController _logicRuleViewController;
        private IRazorEngineService _razorEngineService;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.Disposing += FrameOnDisposing;
            _logicRuleViewController = Frame.GetController<LogicRuleViewController>();
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute += LogicRuleExecutorOnLogicRuleExecute;
            
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing -= FrameOnDisposing;
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute -= LogicRuleExecutorOnLogicRuleExecute;
        }

        void LogicRuleExecutorOnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs logicRuleExecuteEventArgs) {
            var logicRuleInfo = logicRuleExecuteEventArgs.LogicRuleInfo;
            var emailRule = logicRuleInfo.Rule as EmailRule;
            if (emailRule != null && !logicRuleInfo.InvertCustomization) {
                var modelApplicationEmail = (IModelApplicationEmail) Application.Model;
                var emailTemplateObject = EmailTemplateObject(modelApplicationEmail, emailRule, ObjectSpace);
                var modelSmtpClientContext = modelApplicationEmail.Email.SmtpClientContexts.First(emailTemplate => emailTemplate.GetValue<string>("Id") == emailRule.SmtpClientContext);
                var email = CreateEmail(logicRuleInfo, emailRule, modelSmtpClientContext,emailTemplateObject,modelApplicationEmail);
                if (email!=null){
                    var smtpClient = CreateSmtpClient(modelSmtpClientContext);
                    smtpClient.Send(email);
                }
            }
        }

        public IRazorEngineService RazorEngineService{
            get{ return _razorEngineService ??
                       (_razorEngineService = Application.FindModule<EmailModule>().RazorEngineService);
            }
        }

        protected virtual MailMessage CreateEmail(LogicRuleInfo logicRuleInfo, EmailRule emailRule, IModelSmtpClientContext modelSmtpClientContext, IEmailTemplate emailTemplateObject, IModelApplicationEmail modelApplicationEmail){
            var body = RazorEngineService.RunCompile(emailTemplateObject.Body, Guid.NewGuid().ToString(),null, logicRuleInfo.Object);
            var email = new MailMessage{
                IsBodyHtml = true,
                Subject = RazorEngineService.RunCompile(emailTemplateObject.Subject, Guid.NewGuid().ToString(), null, logicRuleInfo.Object),
                Body = body
            };
            if (emailRule.CurrentObjectEmailMember != null){
                var toEmail = emailRule.CurrentObjectEmailMember.MemberInfo.GetValue(logicRuleInfo.Object) as string;
                if (toEmail != null) email.To.Add(toEmail);
            }
            if (!string.IsNullOrEmpty(emailRule.EmailReceipientsContext)) {
                AddReceipients(emailRule, modelApplicationEmail, email,logicRuleInfo.Object);
            }
            email.From = new MailAddress(modelSmtpClientContext.SenderEmail);
            modelSmtpClientContext.ReplyToEmails.Split(';').Each(s => email.ReplyToList.Add(s));
            return email.To.Count == 0 ? null : email;
        }

        protected virtual void AddReceipients(EmailRule emailRule, IModelApplicationEmail modelApplicationEmail, MailMessage email, object currentObject) {
            var emailReceipientGroup =modelApplicationEmail.Email.EmailReceipients.First(
                    @group => @group.GetValue<string>("Id") == emailRule.EmailReceipientsContext);
            foreach (var modelEmailReceipient in emailReceipientGroup) {
                var criteriaOperator = GetCriteriaOperator(modelEmailReceipient,currentObject);
                var objects = ObjectSpace.GetObjects(modelEmailReceipient.EmailReceipient.TypeInfo.Type,                                                     criteriaOperator);
                var sendToCollection = GetSendToCollection(email, modelEmailReceipient);
                foreach (var obj in objects){
                    var item = modelEmailReceipient.EmailMember.MemberInfo.GetValue(obj) as string;
                    if (item != null) sendToCollection.Add(item);
                }
            }
        }

        protected virtual CriteriaOperator GetCriteriaOperator(IModelEmailReceipient modelEmailReceipient, object o) {
            var keyValue = ObjectSpace.GetKeyValue(o);
            modelEmailReceipient.Criteria = modelEmailReceipient.Criteria.Replace(RuleObjectKeyValue, keyValue.ToString());
            var criteriaOperator = CriteriaOperator.Parse(modelEmailReceipient.Criteria);
            return criteriaOperator;
        }

        protected virtual MailAddressCollection GetSendToCollection(MailMessage email, IModelEmailReceipient modelEmailReceipient){
            var collection = email.To;
            return modelEmailReceipient.EmailType != EmailType.Normal
                ? (modelEmailReceipient.EmailType == EmailType.BCC ? email.Bcc : email.CC)
                : collection;
        }

        private  SmtpClient CreateSmtpClient(IModelSmtpClientContext modelSmtpClientContext) {
            var smtpClient = new SmtpClient {
                DeliveryMethod = modelSmtpClientContext.DeliveryMethod,
            };
            if (!string.IsNullOrEmpty(modelSmtpClientContext.PickupDirectoryLocation))
                smtpClient.PickupDirectoryLocation = Path.GetFullPath(modelSmtpClientContext.PickupDirectoryLocation);
            if (smtpClient.DeliveryMethod == SmtpDeliveryMethod.Network){
                smtpClient.Host = modelSmtpClientContext.Host;
                smtpClient.Port = modelSmtpClientContext.Port;
                smtpClient.EnableSsl = modelSmtpClientContext.EnableSsl;
                smtpClient.UseDefaultCredentials = modelSmtpClientContext.UseDefaultCredentials;
                if (!smtpClient.UseDefaultCredentials){
                    smtpClient.Credentials = new NetworkCredential(modelSmtpClientContext.UserName,
                        modelSmtpClientContext.Password);
                }
            }
            else{
                if (!string.IsNullOrEmpty(smtpClient.PickupDirectoryLocation) &&
                    !Directory.Exists(smtpClient.PickupDirectoryLocation)){
                    Directory.CreateDirectory(smtpClient.PickupDirectoryLocation);
                }
            }
            return smtpClient;
        }

        IEmailTemplate EmailTemplateObject(IModelApplicationEmail modelApplication, EmailRule emailRule,
                                                  IObjectSpace objectSpace) {
            var modelEmailTemplate =modelApplication.Email.EmailTemplateContexts.First(
                    emailTemplate => emailTemplate.GetValue<string>("Id") == emailRule.TemplateContext);
            return (IEmailTemplate)objectSpace.FindObject(modelEmailTemplate.EmailTemplate.TypeInfo.Type,
                                       CriteriaOperator.Parse(modelEmailTemplate.Criteria));
        }
    }

}
