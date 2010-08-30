using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace FeatureCenter.Module.ApplicationDifferences
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        protected override string GetTypeToDecorate() {
            return typeof(ModelDifferenceObject).FullName;
        }

        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute("ModelDifferenceObject_ListView", "ModelDifference");
        }
    }
}
