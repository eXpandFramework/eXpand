using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.AuditTrail.Logic;
using Xpand.ExpressApp.AuditTrail.Model;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.AuditTrail.Security {
    public class PopulateAuditTrailMembersContextContextsController : PopulateController<IContextAuditTrailRule> {
        protected override IEnumerable<string> RefreshingProperties() {
            return new[]{((IContextAuditTrailRule) View.CurrentObject).GetPropertyName(rule => rule.TypeInfo)};
        }

        protected override string GetPredefinedValues(IModelMember wrapper) {
            var membersContextGroup = ((IModelApplicationAudiTrail)Application.Model).AudiTrail.AuditTrailMembersContextGroup;
            return string.Join(";", membersContextGroup.Select(contexts => contexts.GetValue<string>("Id")));
        }

        protected override Expression<Func<IContextAuditTrailRule, object>> GetPropertyName() {
            return rule => rule.AuditTrailMembersContext;
        }
    }
}