using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using DevExpress.ExpressApp.Model.Core;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    public abstract class CombineActiveModelDictionaryController<DifferenceStore> : ViewController where DifferenceStore : ModelDifferenceObject
    {
        protected CombineActiveModelDictionaryController()
        {
            TargetObjectType = typeof(DifferenceStore);
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
            var store = (args.Object) as DifferenceStore;
            if (store != null && ReferenceEquals(GetActiveDifference(store.PersistentApplication, Application.GetType().FullName), store)){
                ((ModelApplicationBase)Application.Model).RemoveLayer(((ModelApplicationBase)Application.Model).LastLayer);
                ((ModelApplicationBase)Application.Model).AddLayer(store.Model);
            }
        }

        protected internal abstract DifferenceStore GetActiveDifference(PersistentApplication persistentApplication, string applicationUniqueName);
    }
}