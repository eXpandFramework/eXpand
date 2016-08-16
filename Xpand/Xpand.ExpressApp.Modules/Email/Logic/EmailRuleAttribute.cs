using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Email;
using Xpand.Persistent.Base.General.TypeConverters;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Email.Logic {
    public sealed class EmailRuleAttribute : LogicRuleAttribute, IContextEmailRule {
        public EmailRuleAttribute(string id) : base(id) {
        }

        public string SmtpClientContext { get; set; }

        public string TemplateContext { get; set; }
        public string EmailReceipientsContext { get; set; }

        [TypeConverter(typeof(StringToModelMemberConverter))]
        public string CurrentObjectEmailMember { get; set; }

        IModelMember IEmailRule.CurrentObjectEmailMember { get; set; }
    }
}