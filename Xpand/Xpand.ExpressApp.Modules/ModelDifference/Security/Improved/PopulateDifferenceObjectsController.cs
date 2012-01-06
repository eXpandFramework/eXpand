using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.ModelDifference.Security.Improved {
    public class PopulateDifferenceObjectsController : PopulateController<ModelCombineOperationPermissionData> {
        protected override string GetPredefinedValues(IModelMember wrapper) {
            var session = ((ObjectSpace)ObjectSpace).Session;
            IQueryable<string> queryable = ( new XPQuery<ModelDifferenceObject>(session)).Select(o => o.Name);
            string ret = Enumerable.Aggregate(queryable, "", (current, s) => current + (s + ";"));
            return ret.TrimEnd(';');
        }

        protected override Expression<Func<ModelCombineOperationPermissionData, object>> GetPropertyName() {
            return x => x.Difference;
        }
    }
}