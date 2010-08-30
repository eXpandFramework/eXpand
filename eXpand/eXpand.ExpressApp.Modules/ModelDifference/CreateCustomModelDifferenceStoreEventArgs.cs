using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.Core;

namespace eXpand.ExpressApp.ModelDifference {
    public class CreateCustomModelDifferenceStoreEventArgs : HandledEventArgs
    {
        internal List<ModelApplicationFromStreamStoreBase> ExtraDiffStores = new List<ModelApplicationFromStreamStoreBase>();
        public ModelDifferenceStore Store { get; set; }

        public void AddExtraDiffStore(ModelApplicationFromStreamStoreBase store)
        {
            ExtraDiffStores.Add(store);
        }
    }
}