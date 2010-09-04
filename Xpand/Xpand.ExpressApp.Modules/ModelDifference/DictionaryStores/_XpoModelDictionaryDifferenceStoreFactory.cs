using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores
{
    public class XpoModelDictionaryDifferenceStoreFactory<T> where T:XpoModelDictionaryDifferenceStore
    {
        public virtual T Create(XafApplication xafApplication,bool enableLoading){
            return (T) Activator.CreateInstance(typeof(T),  xafApplication, enableLoading);
        }
    }
}
