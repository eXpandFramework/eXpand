using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Security {
    public class PopulateActionsController : PopulateController<ActionStateRulePermission> {
        protected override string GetPredefinedValues(IModelMember wrapper) {
            return Application.Model.ActionDesign.Actions.Aggregate("", (current, action) => current + (action.Id + ";")).TrimEnd(';');
        }

        protected override Expression<Func<ActionStateRulePermission, object>> GetPropertyName() {
            return x => x.ActionId;
        }
    }
}