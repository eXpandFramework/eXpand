using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.ModelDifference.Core;

namespace Xpand.ExpressApp.ModelDifference {
    public class CreateCustomModelDifferenceStoreEventArgs : HandledEventArgs {
        internal List<ModelApplicationFromStreamStoreBase> ExtraDiffStores = new List<ModelApplicationFromStreamStoreBase>();
        public ModelDifferenceStore Store { get; set; }

        public void AddExtraDiffStore(ModelApplicationFromStreamStoreBase store) {
            ExtraDiffStores.Add(store);
        }
    }
}