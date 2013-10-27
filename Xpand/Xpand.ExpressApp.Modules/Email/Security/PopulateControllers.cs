using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Email.Logic;
using Xpand.ExpressApp.Email.Model;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Email.Security {
    public class PopulateSmtpClientContextController:PopulateController<IContextEmailRule> {
        protected override string GetPredefinedValues(IModelMember wrapper) {
            var smtpClientContexts = ((IModelApplicationEmail)Application.Model).Email.SmtpClientContexts;
            return string.Join(";", smtpClientContexts.Select(contexts => contexts.GetValue<string>("Id")));
        }

        protected override Expression<Func<IContextEmailRule, object>> GetPropertyName() {
            return rule => rule.SmtpClientContext;
        }
    }

    public class PopulateTemplateContextContextController : PopulateController<IContextEmailRule> {
        protected override string GetPredefinedValues(IModelMember wrapper) {
            var templateContexts = ((IModelApplicationEmail)Application.Model).Email.EmailTemplateContexts;
            return string.Join(";", templateContexts.Select(contexts => contexts.GetValue<string>("Id")));
        }

        protected override Expression<Func<IContextEmailRule, object>> GetPropertyName() {
            return rule => rule.TemplateContext;
        }
    }

    public class PopulateEmailReceipientsContextContextController : PopulateController<IContextEmailRule> {
        protected override string GetPredefinedValues(IModelMember wrapper) {
            var emailReceipients = ((IModelApplicationEmail)Application.Model).Email.EmailReceipients;
            return string.Join(";", emailReceipients.Select(contexts => contexts.GetValue<string>("Id")));
        }

        protected override Expression<Func<IContextEmailRule, object>> GetPropertyName() {
            return rule => rule.EmailReceipientsContext;
        }
    }

    public class PopulateCurrentObjectEmailMemberController : PopulateController<IContextEmailRule> {
        protected override IEnumerable<string> RefreshingProperties() {
            return new[] { ((IContextEmailRule)View.CurrentObject).GetPropertyName(rule => rule.TypeInfo) };
        }

        protected override string GetPredefinedValues(IModelMember wrapper) {
            var typeInfo = ((IContextEmailRule) View.CurrentObject).TypeInfo;
            var values=new[]{" "};
            if (typeInfo != null) {   
                values = typeInfo.Members.Select(member => member.Name).ToArray();
            }
            return string.Join(";", values);
        }

        protected override Expression<Func<IContextEmailRule, object>> GetPropertyName() {
            return rule => rule.CurrentObjectEmailMember;
        }
    }
}
