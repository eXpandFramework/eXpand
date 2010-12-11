using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.ModelDifference.Security.Controllers {
    public class PopulateDifferenceObjectsController : PopulateController<ModelCombinePermission> {
        protected override string GetPredefinedValues(IModelMember wrapper) {
            IQueryable<string> queryable = new XPQuery<ModelDifferenceObject>(((ObjectSpace)ObjectSpace).Session).Select(o => o.Name);
            string ret = Enumerable.Aggregate(queryable, "", (current, s) => current + (s + ";"));
            return ret.TrimEnd(';');
        }

        protected override Expression<Func<ModelCombinePermission, object>> GetPropertyName() {
            return x => x.Difference;
        }
    }
}
