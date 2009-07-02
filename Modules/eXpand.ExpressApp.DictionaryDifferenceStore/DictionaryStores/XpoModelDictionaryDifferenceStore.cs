using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.DictionaryStores
{
    public class XpoModelDictionaryDifferenceStore : XpoModelDictionaryDifferenceStoreBase
    {
        public XpoModelDictionaryDifferenceStore(Session session, XafApplication application) : base(session, application) { }

        
        public override DifferenceType DifferenceType
        {
            get { return DifferenceType.Model; }
        }

        protected override BaseObjects.XpoModelDictionaryDifferenceStore getNewStore(Session session)
        {
            return new BaseObjects.XpoModelDictionaryDifferenceStore(session);
        }
    }
}