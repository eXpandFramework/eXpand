namespace FeatureCenter.Module.WorldCreator.DynamicAssemblyMasterDetail
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        protected override string GetTypeToDecorate() {
            return WorldCreatorUpdater.MasterDetailDynamicAssembly+"."+WorldCreatorUpdater.DMDCustomer;
        }

        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute("DMDCustomer_ListView", "WC3LevelMasterDetailModelStore");
        }
    }
}
