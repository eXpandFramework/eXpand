using System;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.Email;
using Xpand.Persistent.BaseImpl.Security.PermissionPolicyData;

namespace Xpand.Persistent.BaseImpl.Email {
    [System.ComponentModel.DisplayName("Email")]
    public class EmailOperationPermissionPolicyData : LogicRuleOperationPermissionPolicyData, IContextEmailRule, IEmailOperationPermissionData {
        public EmailOperationPermissionPolicyData(Session session) : base(session) {
        }

        [RuleRequiredField]
        public string SmtpClientContext { get; set; }

        [RuleRequiredField]
        public string TemplateContext { get; set; }

        public string EmailReceipientsContext { get; set; }        

        public string CurrentObjectEmailMember { get; set; }

        IModelMember IEmailRule.CurrentObjectEmailMember { get; set; }
        protected override Type GetPermissionType(){
            return typeof(IContextEmailRule);
        }
    }
}