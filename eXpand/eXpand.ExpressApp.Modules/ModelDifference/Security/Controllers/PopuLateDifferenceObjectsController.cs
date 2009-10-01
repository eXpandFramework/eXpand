using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.Security.Controllers;
using System.Linq;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.ModelDifference.Security.Controllers
{
    public partial class PopuLateDifferenceObjectsController : PopulateController
    {
        public PopuLateDifferenceObjectsController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string GetPredefinedValues(PropertyInfoNodeWrapper wrapper){
            IQueryable<string> queryable = new XPQuery<ModelDifferenceObject>(ObjectSpace.Session).Select(o => o.Name);
            string ret = "";
            foreach (var s in queryable){
                ret += s + ";";
            }
            return ret.TrimEnd(';');
        }

        protected override string GetPermissionPropertyName(){
            return new ModelCombinePermission().GetPropertyInfo(x => x.Difference).Name;
        }
    }
}
