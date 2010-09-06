using System;
using System.Linq.Expressions;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.Security.Controllers;
using System.Linq;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.ModelDifference.Security.Controllers
{
    public class PopulateDifferenceObjectsController : PopulateController<ModelCombinePermission>
    {
        protected override string GetPredefinedValues(IModelMember wrapper){
            IQueryable<string> queryable = new XPQuery<ModelDifferenceObject>(ObjectSpace.Session).Select(o => o.Name);
            string ret = Enumerable.Aggregate(queryable, "", (current, s) => current + (s + ";"));
            return ret.TrimEnd(';');
        }

        protected override Expression<Func<ModelCombinePermission, object>> GetPropertyName(){
            return x => x.Difference;
        }
    }
}
