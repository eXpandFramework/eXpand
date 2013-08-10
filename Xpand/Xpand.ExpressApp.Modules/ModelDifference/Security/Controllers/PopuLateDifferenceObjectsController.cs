using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.ModelDifference.Security.Controllers {
    public class PopulateDifferenceObjectsController : PopulateController<IModelCombinePermission> {
        protected override string GetPredefinedValues(IModelMember wrapper) {
            IQueryable<string> queryable = new XPQuery<ModelDifferenceObject>(((XPObjectSpace)ObjectSpace).Session).Select(o => o.Name);
            string ret = Enumerable.Aggregate(queryable, "", (current, s) => current + (s + ";"));
            return ret.TrimEnd(';');
        }

        protected override Expression<Func<IModelCombinePermission, object>> GetPropertyName() {
            return x => x.Difference;
        }
    }
}
