using System.IO;
using Xpand.Persistent.Base.ModelDifference;

namespace FeatureCenter.Module.Win.ApplicationDifferences.ExternalApplication {
    public class ExternalApplicationModelStore : ModelApplicationFromStreamStoreBase {
        public override string Name {
            get { return "ExternalApplication.Win.ExternalApplicationWindowsFormsApplication"; }
        }
        protected override Stream GetStream() {
            return GetType().Assembly.GetManifestResourceStream(GetType(), "ExternalApplication.xafml");
        }
    }
}