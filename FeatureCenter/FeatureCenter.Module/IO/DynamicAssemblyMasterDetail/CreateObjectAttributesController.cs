using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.IO.DynamicAssemblyMasterDetail
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        protected override string GetTypeToDecorate() {
            return WorldCreatorUpdater.MasterDetailDynamicAssembly+"."+WorldCreatorUpdater.DMDCustomer;
        }

        protected override NavigationItemAttribute GetNavigationItemAttribute() {
            return new NavigationItemAttribute(Captions.IO + "Dynamic assembly Master detail", "IODMDCustomer_ListView");
        }

        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute("IODMDCustomer_ListView", "IOWC3LevelMasterDetailModelStore");
        }
    }
}
