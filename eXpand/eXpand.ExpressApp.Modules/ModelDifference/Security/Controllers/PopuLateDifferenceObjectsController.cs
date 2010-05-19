using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.Security.Controllers;
using System.Linq;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.ModelDifference.Security.Controllers
{
    public class PopulateDifferenceObjectsController : PopulateController<ModelCombinePermission>
    {
        public PopulateDifferenceObjectsController() {
        }

        protected override string GetPredefinedValues(IModelMember wrapper){
            IQueryable<string> queryable = new XPQuery<ModelDifferenceObject>(ObjectSpace.Session).Select(o => o.Name);
            string ret = "";
            foreach (var s in queryable){
                ret += s + ";";
            }
            return ret.TrimEnd(';');
        }

        protected override Expression<Func<ModelCombinePermission, object>> GetPropertyName(){
            return x => x.Difference;
        }
    }
}
