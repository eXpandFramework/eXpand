using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Logic.Security.Improved;
using Xpand.Persistent.Base.Email;

namespace Xpand.ExpressApp.Email.Security {
    [System.ComponentModel.DisplayName("Email")]
    public class EmailOperationPermissionData : LogicRuleOperationPermissionData, IContextEmailRule, IEmailOperationPermissionData{
        public EmailOperationPermissionData(Session session) : base(session) {
        }

        [RuleRequiredField]
        public string SmtpClientContext { get; set; }

        [RuleRequiredField]
        public string TemplateContext { get; set; }

        public string EmailReceipientsContext { get; set; }

        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[]{new EmailRulePermission(this)};
        }
        

        public string CurrentObjectEmailMember { get; set; }

        IModelMember IEmailRule.CurrentObjectEmailMember { get; set; }
    }
}