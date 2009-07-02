using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.DictionaryStores
{
    public class CustomDictionaryDifferenceStore : DevExpress.ExpressApp.DictionaryDifferenceStore
    {
        private readonly BaseObjects.XpoModelDictionaryDifferenceStore store;

        public CustomDictionaryDifferenceStore(BaseObjects.XpoModelDictionaryDifferenceStore store)
        {
            this.store = store;
        }

        protected override Dictionary LoadDifferenceCore(Schema schema)
        {
            DictionaryNode rootNode = null;
            if (store != null)
                rootNode = new DictionaryXmlReader().ReadFromString(store.XmlContent);
            if (rootNode == null)
                rootNode = new DictionaryNode("Application");
            return new Dictionary(rootNode, schema);
        }

        public override string Name
        {
            get { return GetType().Name; }
        }

        public override void SaveDifference(Dictionary diffDictionary)
        {
            throw new NotImplementedException();
        }
    }
}