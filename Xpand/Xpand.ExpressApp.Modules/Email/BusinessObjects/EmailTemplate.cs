using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Email.BusinessObjects {
    [DefaultClassOptions]
    public class EmailTemplate : XpandCustomObject, IEmailTemplate {
        string _body;
        string _name;
        string _subject;

        public EmailTemplate(Session session) : base(session) {
        }

        [EditorAlias(EditorAliases.HtmlPropertyEditor), RuleRequiredField, Size(-1)]
        [Index(2)]
        public string Body {
            get { return _body; }
            set { SetPropertyValue("Body", ref _body, value); }
        }

        [RuleRequiredField, RuleUniqueValue(DefaultContexts.Save)]
        [Index(0)]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [ImmediatePostData, RuleRequiredField]
        [Index(1)]
        public string Subject {
            get { return _subject; }
            set {
                SetPropertyValue("Subject", ref _subject, value);
                if ((string.IsNullOrEmpty(Name)) && IsNewObject) {
                    Name = value;
                }
            }
        }
    }
}