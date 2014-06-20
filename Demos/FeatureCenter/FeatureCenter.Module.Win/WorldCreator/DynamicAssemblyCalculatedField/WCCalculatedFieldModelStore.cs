using System.IO;
using Xpand.Persistent.Base.ModelDifference;

namespace FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyCalculatedField {
    public class WCCalculatedFieldModelStore : ModelApplicationFromStreamStoreBase {
        protected override Stream GetStream() {
            return GetType().Assembly.GetManifestResourceStream(GetType(), "WCCalculatedField.xafml");
        }
    }
}