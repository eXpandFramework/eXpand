using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace FeatureCenter.Module.ApplicationDifferences.ExternalApplication
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        protected override string GetTypeToDecorate() {
            return typeof(ModelDifferenceObject).FullName;
        }

        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute("ExternalApplication_DetailView", "ExternalApplication");
        }

    }
}
