
namespace Xpand.ExpressApp.Email.BusinessObjects {
    public interface IEmailTemplate {
        string Name { get; set; }
        string Subject { get; set; }
        string Body { get; }
    }
}