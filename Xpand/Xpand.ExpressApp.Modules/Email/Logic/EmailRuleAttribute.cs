using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.TypeConverters;

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