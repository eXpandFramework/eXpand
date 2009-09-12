using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.Win.Controllers
{
    public partial class CombineActiveModelDictionaryWithActiveUserDifferenceController : CombineActiveModelDictionaryController<UserModelDifferenceObject>
    {
        public CombineActiveModelDictionaryWithActiveUserDifferenceController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override UserModelDifferenceObject GetActiveDifference(PersistentApplication persistentApplication,IApplicationUniqueName applicationUniqueName){
            return new QueryUserModelDifferenceObject(View.ObjectSpace.Session).GetActiveModelDifference(
                applicationUniqueName.UniqueName);
        }
    }
}