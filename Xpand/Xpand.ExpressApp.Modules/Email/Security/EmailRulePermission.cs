using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using Xpand.ExpressApp.Email.Logic;
using Xpand.ExpressApp.Logic.Security.Improved;

namespace Xpand.ExpressApp.Email.Security {
    public class EmailRulePermission : LogicRulePermission, IContextEmailRule{
        public const string OperationName = "Email";

        public EmailRulePermission(EmailOperationPermissionData contextLogicRule) : base(OperationName, contextLogicRule) {
            TemplateContext = contextLogicRule.TemplateContext;
            SmtpClientContext = contextLogicRule.SmtpClientContext;
            CurrentObjectEmailMember =CaptionHelper.ApplicationModel.BOModel.GetClass(ObjectType)
                             .FindMember(contextLogicRule.CurrentObjectEmailMember);
        }

        public string SelectedObjectEmailMember { get; set; }

        public string SmtpClientContext { get; set; }

        public string TemplateContext { get; set; }

        public override IList<string> GetSupportedOperations() {
            return new[]{OperationName};
        }

        public IModelMember CurrentObjectEmailMember { get; set; }
    }
}