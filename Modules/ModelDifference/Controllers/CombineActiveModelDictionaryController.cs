using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

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
            ObjectSpace.ObjectSaved+=ObjectSpaceOnObjectSaved;
        }

        internal void ObjectSpaceOnObjectSaved(object sender, ObjectManipulatingEventArgs args){
            var store = (args.Object) as DifferenceStore;
            if (store != null && ReferenceEquals(GetActiveDifference(store.PersistentApplication, Application.Title), store)){
                var combiner = new DictionaryCombiner(Application.Model);
                combiner.AddAspects(store.Model);
            }
        }

        protected internal abstract DifferenceStore GetActiveDifference(PersistentApplication persistentApplication, string applicationUniqueName);

        
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            ObjectSpace.ObjectSaved -= ObjectSpaceOnObjectSaved;
        }

    }
}