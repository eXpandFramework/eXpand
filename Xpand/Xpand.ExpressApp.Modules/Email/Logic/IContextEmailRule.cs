using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Email.Logic {
    public interface IContextEmailRule : IEmailRule, IContextLogicRule {
        [Required, Category("Email"), DataSourceProperty("SmtpClientContexts")]
        string SmtpClientContext { get; set; }

        [Category("Email"), DataSourceProperty("TemplateContexts"),Required]
        string TemplateContext { get; set; }

        [Category("Email")]
        [DataSourceProperty("EmailReceipientsContexts")]
        [RuleRequiredField(TargetCriteria = "CurrentObjectEmailMember is null")]
        string EmailReceipientsContext { get; set; }
    }
}