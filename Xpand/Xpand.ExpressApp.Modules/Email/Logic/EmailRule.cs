using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Email.Logic {
    public class EmailRule : LogicRule, IEmailRule {
        public EmailRule(IContextEmailRule emailRule) : base(emailRule) {
            SmtpClientContext = emailRule.SmtpClientContext;
            TemplateContext = emailRule.TemplateContext;
            CurrentObjectEmailMember=emailRule.CurrentObjectEmailMember;
            EmailReceipientsContext = emailRule.EmailReceipientsContext;
        }

        public string EmailReceipientsContext { get; set; }

        public string SmtpClientContext { get; set; }

        public string TemplateContext { get; set; }
        public IModelMember CurrentObjectEmailMember { get; set; }
    }
}