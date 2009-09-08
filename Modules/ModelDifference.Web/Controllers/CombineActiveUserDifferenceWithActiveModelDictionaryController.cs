using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace eXpand.ExpressApp.ModelDifference.Web.Controllers
{
    public partial class CombineActiveModelDictionaryWithActiveModelDifferenceController : CombineActiveModelDictionaryController<ModelDifferenceObject>
    {
        public CombineActiveModelDictionaryWithActiveModelDifferenceController()
        {
            InitializeComponent();
            RegisterActions(components);
        }


        protected override ModelDifferenceObject GetActiveDifference(string applicationName){
            return new QueryModelDifferenceObject(View.ObjectSpace.Session).GetActiveModelDifference(applicationName);
        }
    }
}