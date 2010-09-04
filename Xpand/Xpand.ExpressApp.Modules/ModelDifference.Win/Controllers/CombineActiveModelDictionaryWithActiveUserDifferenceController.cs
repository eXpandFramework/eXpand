using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace eXpand.ExpressApp.ModelDifference.Win.Controllers
{
    public class CombineActiveModelDictionaryWithActiveUserDifferenceController : CombineActiveModelDictionaryController<ModelDifferenceObject>
    {
        protected override ModelDifferenceObject GetActiveDifference(PersistentApplication persistentApplication, string applicationUniqueName){
            return new QueryModelDifferenceObject(View.ObjectSpace.Session)
                .GetActiveModelDifference(applicationUniqueName);
        }
    }
}