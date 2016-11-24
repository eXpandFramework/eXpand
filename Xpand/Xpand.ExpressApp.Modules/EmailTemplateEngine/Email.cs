namespace Xpand.EmailTemplateEngine {
    using System.Collections.Generic;
    using System.Diagnostics;

    public class Email {
        [DebuggerStepThrough]
        public Email() {
            To = new List<string>();
            ReplyTo = new List<string>();
            CC = new List<string>();
            Bcc = new List<string>();
            Headers = new Dictionary<string, string>();
            Attachments = new Dictionary<string, byte[]>();
        }

        public string From { get; set; }

        public string Sender { get; set; }

        public ICollection<string> To { get; private set; }

        public ICollection<string> ReplyTo { get; private set; }

        public ICollection<string> CC { get; private set; }

        public ICollection<string> Bcc { get; private set; }

        public IDictionary<string, string> Headers { get; private set; }

        public string Subject { get; set; }

        public string HtmlBody { get; set; }

        public string TextBody { get; set; }

        public Dictionary<string, byte[]> Attachments { get; set; }
    }
}