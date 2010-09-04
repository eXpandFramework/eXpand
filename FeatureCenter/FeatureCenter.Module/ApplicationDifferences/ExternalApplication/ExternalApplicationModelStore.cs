using System.IO;

namespace FeatureCenter.Module.ApplicationDifferences.ExternalApplication {
    public class ExternalApplicationModelStore : Xpand.ExpressApp.ModelDifference.Core.ModelApplicationFromStreamStoreBase
    {
        public override string Name {
            get { return "ExternalApplication.Win.ExternalApplicationWindowsFormsApplication"; }
        }
        protected override Stream GetStream() {
            return GetType().Assembly.GetManifestResourceStream(GetType(), "ExternalApplication.xafml");
        }
    }
}