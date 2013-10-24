using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Email.Logic;
using Xpand.ExpressApp.Logic.Security.Improved;

namespace Xpand.ExpressApp.Email.Security {
    public class EmailOperationPermissionData : LogicRuleOperationPermissionData, IContextEmailRule{
        public EmailOperationPermissionData(Session session) : base(session) {
        }

        [RuleRequiredField]
        public string SmtpClientContext { get; set; }

        [RuleRequiredField]
        public string TemplateContext { get; set; }

        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[]{new EmailRulePermission(this)};
        }
        

        public string CurrentObjectEmailMember { get; set; }

        IModelMember IEmailRule.CurrentObjectEmailMember { get; set; }
    }
}