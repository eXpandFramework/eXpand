using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelDifference {
    public class CreateCustomModelDifferenceStoreEventArgs : HandledEventArgs
    {
        internal List<ModelFromResourceStoreBase> ExtraDiffStores = new List<ModelFromResourceStoreBase>();
        public ModelDifferenceStore Store { get; set; }

        public void AddExtraDiffStore(ModelFromResourceStoreBase store)
        {
            ExtraDiffStores.Add(store);
        }
    }
}