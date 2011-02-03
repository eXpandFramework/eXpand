using System.IO;
using Xpand.ExpressApp.ModelDifference.Core;

namespace FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyMasterDetail {
    public class WC3LevelMasterDetailModelStore : ModelApplicationFromStreamStoreBase {
        protected override Stream GetStream() {
            return GetType().Assembly.GetManifestResourceStream(GetType(), "WC3LevelMasterDetail.xafml");
        }
    }
}