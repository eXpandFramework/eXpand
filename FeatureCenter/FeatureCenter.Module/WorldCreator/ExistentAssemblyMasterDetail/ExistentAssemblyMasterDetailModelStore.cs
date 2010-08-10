using System.IO;
using eXpand.ExpressApp.ModelDifference.Core;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail {
    public class ExistentAssemblyMasterDetailModelStore : ModelApplicationFromStreamStoreBase
    {
        protected override Stream GetStream() {
            return GetType().Assembly.GetManifestResourceStream(GetType(), "ExisstentAssemblyMasterDetail.xafml");
        }
    }
}