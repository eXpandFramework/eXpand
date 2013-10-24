using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Email.Logic {
    public interface IContextEmailRule : IEmailRule, IContextLogicRule {
        [Required, Category("Email"), DataSourceProperty("SmtpClientContexts")]
        string SmtpClientContext { get; set; }

        [Category("Email"), DataSourceProperty("TemplateContexts")]
        string TemplateContext { get; set; }
    }
}