using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.ImportExport.DynamicAssemblyMasterDetail
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        protected override string GetTypeToDecorate() {
            return WorldCreatorUpdater.MasterDetailDynamicAssembly+"."+WorldCreatorUpdater.DMDCustomer;
        }

        protected override XpandNavigationItemAttribute GetNavigationItemAttribute() {
            return new XpandNavigationItemAttribute(Captions.Importexport + "Dynamic assembly Master detail", "IODMDCustomer_ListView");
        }

        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute("IODMDCustomer_ListView", "IOWC3LevelMasterDetail");
        }
    }
}
