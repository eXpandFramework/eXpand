using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace EmailTester.Module.FunctionalTests {
    [DefaultClassOptions]
    public class EmailObject : BaseObject {
        public EmailObject(Session session)
            : base(session) {
        }

        public string From { get; set; }

        public string To { get; set; }

        public string ReplyTo { get; set; }

        public string Subject { get; set; }
        [Size(SizeAttribute.Unlimited)]
        public string Body { get; set; }
    }
}

