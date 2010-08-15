using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.ImportExport.AnalysisObjects
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        private const string ViewId = "IOAnalysis_ListView";
        protected override string GetTypeToDecorate() {
            return typeof(DevExpress.Persistent.BaseImpl.Analysis).FullName;
        }
        protected override CloneViewAttribute GetCloneViewAttribute()
        {
            return new CloneViewAttribute(CloneViewType.ListView, ViewId);
        }
        protected override NavigationItemAttribute GetNavigationItemAttribute()
        {
            return new NavigationItemAttribute(Captions.Importexport+ "Analysis", ViewId);
        }
        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute(ViewId, "Analysis");
        }
    }
}
