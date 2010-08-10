using System.IO;
using eXpand.ExpressApp.ModelDifference.Core;

namespace FeatureCenter.Module.IO.DynamicAssemblyMasterDetail {
    public class IOWC3LevelMasterDetailModelStore : ModelApplicationFromStreamStoreBase
    {
        protected override Stream GetStream() {
            return GetType().Assembly.GetManifestResourceStream(GetType(), "IOWC3LevelMasterDetail.xafml");
        }
    }
}