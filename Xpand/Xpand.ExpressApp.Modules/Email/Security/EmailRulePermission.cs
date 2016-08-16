using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using Fasterflect;
using Xpand.ExpressApp.Logic.Security.Improved;
using Xpand.Persistent.Base.Email;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Email.Security {
    public class EmailRulePermission : LogicRulePermission, IContextEmailRule{
        public const string OperationName = "Email";

        public EmailRulePermission(IContextEmailRule contextLogicRule) : base(OperationName, contextLogicRule) {
            EmailReceipientsContext = contextLogicRule.EmailReceipientsContext;
            TemplateContext = contextLogicRule.TemplateContext;
            SmtpClientContext = contextLogicRule.SmtpClientContext;
            var objectTypeData = (Type)contextLogicRule.GetPropertyValue(nameof(ILogicRuleOperationPermissionData.ObjectTypeData));
            if (objectTypeData != null){
                var propertyValue = (string) contextLogicRule.GetPropertyValue(nameof(IEmailOperationPermissionData.CurrentObjectEmailMember));
                if (!string.IsNullOrWhiteSpace(propertyValue))
                    CurrentObjectEmailMember =CaptionHelper.ApplicationModel.BOModel.GetClass(objectTypeData).FindMember(propertyValue);
            }
        }

        public string SelectedObjectEmailMember { get; set; }

        public string SmtpClientContext { get; set; }

        public string TemplateContext { get; set; }
        public string EmailReceipientsContext { get; set; }

        public override IList<string> GetSupportedOperations() {
            return new[]{OperationName};
        }

        public IModelMember CurrentObjectEmailMember { get; set; }
    }
}