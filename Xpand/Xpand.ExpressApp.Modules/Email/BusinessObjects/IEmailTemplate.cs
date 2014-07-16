using Xpand.EmailTemplateEngine;

namespace Xpand.ExpressApp.Email.BusinessObjects {
    public interface IEmailTemplate:IEmail {
        string Name { get; set; }
    }
}