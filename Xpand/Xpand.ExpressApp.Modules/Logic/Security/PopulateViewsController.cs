using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.Logic.Security {
    public class PopulateViewsController : PopulateController<LogicRulePermission> {
        protected override string GetPredefinedValues(IModelMember wrapper) {
            string ret = Application.Model.Views.Aggregate("", (current, view) => current + (view.Id + ";"));
            ret = ret.TrimEnd(';');
            return ret;
        }

        protected override Expression<Func<LogicRulePermission, object>> GetPropertyName() {
            return x => x.ViewId;
        }
    }
}