using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Email.BusinessObjects {
    [DefaultClassOptions]
    [ImageName("Action_WindowList")]
    public class EmailTemplate : XpandCustomObject, IEmailTemplate {
        string _body;
        string _name;
        string _subject;

        public EmailTemplate(Session session) : base(session) {
        }
        public void Configure(EmailTemplateConfig config, string acivationHost=null) {
            if (config == EmailTemplateConfig.UserActivation){
                Subject = "User activation";
                Body = string.Format("A new user @Model.User.UserName has been created. To activate the account please click the following link {0}@Model.User.Activation",
                                                   acivationHost + "?ua=");
            }
            else{
                Subject = "pass forgotten";
                Body = "We created a temporary password (@Model.Password) for the UserName (@Model.User.UserName). Please login to reset it";
            }
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

    public enum EmailTemplateConfig{
        UserActivation,
        PassForgotten
    }
}