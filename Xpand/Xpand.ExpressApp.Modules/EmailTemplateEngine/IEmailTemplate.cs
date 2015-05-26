namespace Xpand.EmailTemplateEngine {
    using System.Collections.Generic;

    public interface IEmail{
        string Subject { get; set; }
        string Body { get; }
    }

    public interface IEmailTemplate : IEmail{
        string From { get; set; }

        string Sender { get; set; }

        ICollection<string> To { get; }

        ICollection<string> ReplyTo { get; }

        ICollection<string> CC { get; }

        ICollection<string> Bcc { get; }

        IDictionary<string, string> Headers { get; }

        IDictionary<string, byte[]> Attachments { get; }

        void SetModel(dynamic model);

        void Execute();
    }
}