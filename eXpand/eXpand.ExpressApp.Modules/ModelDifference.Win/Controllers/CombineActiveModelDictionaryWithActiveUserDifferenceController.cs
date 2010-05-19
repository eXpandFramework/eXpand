using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace eXpand.ExpressApp.ModelDifference.Win.Controllers
{
    public class CombineActiveModelDictionaryWithActiveUserDifferenceController : CombineActiveModelDictionaryController<UserModelDifferenceObject>
    {
        public CombineActiveModelDictionaryWithActiveUserDifferenceController() {
        }

        protected override UserModelDifferenceObject GetActiveDifference(PersistentApplication persistentApplication, string applicationUniqueName){
            return new QueryUserModelDifferenceObject(View.ObjectSpace.Session)
                .GetActiveModelDifference(applicationUniqueName);
        }
    }
}