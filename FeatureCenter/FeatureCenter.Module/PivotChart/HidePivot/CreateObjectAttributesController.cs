using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.PivotChart.HidePivot
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        private const string DetailView = "HidePivot_DetailView";
        protected override string GetTypeToDecorate() {
            return typeof(Analysis).FullName;
        }
        protected override CloneViewAttribute GetCloneViewAttribute()
        {
            return new CloneViewAttribute(CloneViewType.DetailView, DetailView);
        }
        protected override XpandNavigationItemAttribute GetNavigationItemAttribute()
        {
            return new XpandNavigationItemAttribute("PivotChart/Hide Pivot", DetailView) { ObjectKey = "Name='HidePivot'" };
        }
        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute(DetailView, new BinaryOperator("Name", "HidePivot"));
        }
    }
}
