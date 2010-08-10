using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.PivotChart.PivotGroupInterval
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        private const string DetailView = "PivotGroupInterval_DetailView";
        protected override string GetTypeToDecorate() {
            return typeof(Analysis).FullName;
        }
        protected override CloneViewAttribute GetCloneViewAttribute()
        {
            return new CloneViewAttribute(CloneViewType.DetailView, DetailView);
        }
        protected override NavigationItemAttribute GetNavigationItemAttribute()
        {
            return new NavigationItemAttribute("PivotChart/Pivot Group Interval", DetailView) { ObjectKey = "Name='PivotGroupInterval'" };
        }
        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute(DetailView, new BinaryOperator("Name", "PivotGroupInterval"));
        }
    }
}
