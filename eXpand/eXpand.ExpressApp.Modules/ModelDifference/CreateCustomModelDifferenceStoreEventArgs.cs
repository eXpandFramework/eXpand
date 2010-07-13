using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelDifference {
    public class CreateCustomModelDifferenceStoreEventArgs : HandledEventArgs
    {
        internal List<KeyValuePair<string, ModelStoreBase>> ExtraDiffStores = new List<KeyValuePair<string, ModelStoreBase>>();
        public ModelDifferenceStore Store { get; set; }

        public void AddExtraDiffStore(string id, ModelStoreBase store)
        {
            ExtraDiffStores.Add(new KeyValuePair<string, ModelStoreBase>(id, store));
        }
    }
}