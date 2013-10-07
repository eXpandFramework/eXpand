using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Security.Improved {
    public class PopulateActionsController : PopulateController<IContextActionStateRule> {
        protected override string GetPredefinedValues(IModelMember wrapper) {
            return Application.Model.ActionDesign.Actions.Aggregate("", (current, action) => current + (action.Id + ";")).TrimEnd(';');
        }

        protected override Expression<Func<IContextActionStateRule, object>> GetPropertyName() {
            return x => x.ActionId;
        }
    }
}