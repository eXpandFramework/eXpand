using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Xpand.EmailTemplateEngine;
using Xpand.ExpressApp.Email.Model;
using Xpand.ExpressApp.Logic;
using System.Linq;
using Xpand.Utils.Helpers;
using IEmailTemplate = Xpand.ExpressApp.Email.BusinessObjects.IEmailTemplate;

namespace Xpand.ExpressApp.Email.Logic {
    public class EmailRuleViewController : ViewController {
        public const string RuleObjectKeyValue = "RuleObjectKeyValue";
        LogicRuleViewController _logicRuleViewController;

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
                if (email!=null) {
                    var emailSender = new EmailSender{
                        CreateClientFactory =
                            () => new SmtpClientWrapper(CreateSmtpClient(modelSmtpClientContext))
                    };
                    emailSender.Send(email);
                }
            }
        }

        protected virtual EmailTemplateEngine.Email CreateEmail(LogicRuleInfo logicRuleInfo, EmailRule emailRule, IModelSmtpClientContext modelSmtpClientContext, IEmailTemplate emailTemplateObject, IModelApplicationEmail modelApplicationEmail) {
            var templateEngine = new EmailTemplateEngine.EmailTemplateEngine(emailTemplateObject);
            var email = templateEngine.Execute(logicRuleInfo.Object, emailRule.ID);
            if (emailRule.CurrentObjectEmailMember != null) {
                var toEmail = emailRule.CurrentObjectEmailMember.MemberInfo.GetValue(logicRuleInfo.Object) as string;
                email.To.Add(toEmail);
            }
            if (!string.IsNullOrEmpty(emailRule.EmailReceipientsContext)) {
                AddReceipients(emailRule, modelApplicationEmail, email,logicRuleInfo.Object);
            }
            email.From = modelSmtpClientContext.SenderEmail;
            modelSmtpClientContext.ReplyToEmails.Split(';').Each(s => email.ReplyTo.Add(s));
            return email.To.Count == 0 ? null : email;
        }

        protected virtual void AddReceipients(EmailRule emailRule, IModelApplicationEmail modelApplicationEmail, EmailTemplateEngine.Email email, object o) {
            var emailReceipientGroup =modelApplicationEmail.Email.EmailReceipients.First(
                    @group => @group.GetValue<string>("Id") == emailRule.EmailReceipientsContext);
            foreach (var modelEmailReceipient in emailReceipientGroup) {
                var criteriaOperator = GetCriteriaOperator(modelEmailReceipient,o);
                var objects = ObjectSpace.GetObjects(modelEmailReceipient.EmailReceipient.TypeInfo.Type,                                                     criteriaOperator);
                var sendToCollection = GetSendToCollection(email, modelEmailReceipient);
                foreach (var obj in objects) {
                    var item = modelEmailReceipient.EmailMember.MemberInfo.GetValue(obj) as string;
                    sendToCollection.Add(item);
                }
            }
        }

        protected virtual CriteriaOperator GetCriteriaOperator(IModelEmailReceipient modelEmailReceipient, object o) {
            var keyValue = ObjectSpace.GetKeyValue(o);
            modelEmailReceipient.Criteria = modelEmailReceipient.Criteria.Replace(RuleObjectKeyValue, keyValue.ToString());
            var criteriaOperator = CriteriaOperator.Parse(modelEmailReceipient.Criteria);
            return criteriaOperator;
        }

        protected virtual ICollection<string> GetSendToCollection(EmailTemplateEngine.Email email, IModelEmailReceipient modelEmailReceipient) {
            var collection = email.To;
            if (modelEmailReceipient.EmailType != EmailType.Normal)
                collection = modelEmailReceipient.EmailType == EmailType.BCC ? email.Bcc : email.CC;
            return collection;
        }

        private  SmtpClient CreateSmtpClient(IModelSmtpClientContext modelSmtpClientContext) {
            var smtpClient = new SmtpClient {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = modelSmtpClientContext.Host,
                Port = modelSmtpClientContext.Port,
                EnableSsl = modelSmtpClientContext.EnableSsl,
                UseDefaultCredentials = modelSmtpClientContext.UseDefaultCredentials
            };
            if (!smtpClient.UseDefaultCredentials) {
                smtpClient.Credentials = new NetworkCredential(
                                    modelSmtpClientContext.UserName,
                                    modelSmtpClientContext.Password);                
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
