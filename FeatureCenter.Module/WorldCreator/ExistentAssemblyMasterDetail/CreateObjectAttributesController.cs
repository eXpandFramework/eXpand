namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        protected override string GetTypeToDecorate() {
            return typeof(EAMDCustomer).FullName;
        }

        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute("EAMDCustomer_ListView", "ExistentAssemblyMasterDetailModelStore");
        }
    }
}
