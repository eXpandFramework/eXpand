using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores
{
    public class XpoModelDictionaryDifferenceStoreFactory<T> where T:XpoModelDictionaryDifferenceStore
    {
        public virtual T Create(Session session,XafApplication xafApplication,bool enableLoading){
            return (T) Activator.CreateInstance(typeof(T), session, xafApplication, enableLoading);
        }
    }
}
