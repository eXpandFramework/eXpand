using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.PivotChart.HideChart
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        private const string DetailView = "HideChart_DetailView";
        protected override string GetTypeToDecorate() {
            return typeof(Analysis).FullName;
        }
        protected override CloneViewAttribute GetCloneViewAttribute()
        {
            return new CloneViewAttribute(CloneViewType.DetailView, DetailView);
        }
        protected override NavigationItemAttribute GetNavigationItemAttribute()
        {
            return new NavigationItemAttribute("PivotChart/Hide Chart", DetailView) { ObjectKey = "Name='HideChart'" };
        }
        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute(DetailView, new BinaryOperator("Name", "HideChart"));
        }
    }
}
