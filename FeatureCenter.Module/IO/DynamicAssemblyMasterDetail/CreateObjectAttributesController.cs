namespace FeatureCenter.Module.IO.DynamicAssemblyMasterDetail
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        protected override string GetTypeToDecorate() {
            return WorldCreatorUpdater.MasterDetailDynamicAssembly+"."+WorldCreatorUpdater.DMDCustomer;
        }

        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute("IODMDCustomer_ListView", "IOWC3LevelMasterDetailModelStore");
        }
    }
}
