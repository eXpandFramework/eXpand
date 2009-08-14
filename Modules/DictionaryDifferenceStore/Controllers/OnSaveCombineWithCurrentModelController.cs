using DevExpress.ExpressApp;
using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Controllers
{
    public abstract class OnSaveCombineWithCurrentModelController<DifferenceStore> : ViewController where DifferenceStore : XpoModelDictionaryDifferenceStore
    {
        protected OnSaveCombineWithCurrentModelController()
        {
            TargetObjectType = typeof(DifferenceStore);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.ObjectSaving+=ObjectSpaceOnObjectSaving;
        }

        private void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs args)
        {
            var store = (args.Object) as XpoModelDictionaryDifferenceStore;
            if (store != null)
            {
                XpoModelDictionaryDifferenceStore activeStore =
                    store.DifferenceType == DifferenceType.User
                        ? XpoUserModelDictionaryDifferenceStoreBuilder.GetActiveStore(store.Session,
                                                                                      store.DifferenceType,
                                                                                      Application.GetType().FullName)
                        : XpoModelDictionaryDifferenceStoreBuilder.GetActiveStore(store.Session, store.DifferenceType,
                                                                                  Application.GetType().FullName);
                if (store.Equals(activeStore))
                    Application.Model.AddAspect(store.Aspect, store.Model);
            }
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            ObjectSpace.ObjectSaving-=ObjectSpaceOnObjectSaving;
        }

    }
}
