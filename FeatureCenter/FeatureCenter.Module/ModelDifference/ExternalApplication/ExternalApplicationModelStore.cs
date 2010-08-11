using System.IO;

namespace FeatureCenter.Module.ModelDifference.ExternalApplication {
    public class ExternalApplicationModelStore : eXpand.ExpressApp.ModelDifference.Core.ModelApplicationFromStreamStoreBase
    {
        public override string Name {
            get { return "ExternalApplication.Win.ExternalApplicationWindowsFormsApplication"; }
        }
        protected override Stream GetStream() {
            return GetType().Assembly.GetManifestResourceStream(GetType(), "ExternalApplication.xafml");
        }
    }
}