using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    public class CombineActiveModelDictionaryController: ViewController 
    {
        public CombineActiveModelDictionaryController()
        {
            TargetObjectType = typeof(ModelDifferenceObject);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.ObjectSaved += ObjectSpaceOnObjectSaved;
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            ObjectSpace.ObjectSaved -= ObjectSpaceOnObjectSaved;
        }

        internal void ObjectSpaceOnObjectSaved(object sender, ObjectManipulatingEventArgs args){
            var store = (args.Object) as ModelDifferenceObject;
            if (store != null && ReferenceEquals(GetActiveDifference(store.PersistentApplication, Application.GetType().FullName), store)){
                ((ModelApplicationBase)Application.Model).RemoveLayer(((ModelApplicationBase)Application.Model).LastLayer);
                ((ModelApplicationBase)Application.Model).AddLayer(store.Model);
            }
        }

        protected virtual ModelDifferenceObject GetActiveDifference(PersistentApplication persistentApplication, string applicationUniqueName) {
            return new QueryModelDifferenceObject(View.ObjectSpace.Session).GetActiveModelDifference(persistentApplication.UniqueName);

        }
    }
}