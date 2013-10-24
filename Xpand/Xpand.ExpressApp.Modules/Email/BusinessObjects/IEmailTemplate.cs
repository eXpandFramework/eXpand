namespace Xpand.ExpressApp.Email.BusinessObjects {
    public interface IEmailTemplate {
        string Body { get; set; }
        string Name { get; set; }
        string Subject { get; set; }
    }
}