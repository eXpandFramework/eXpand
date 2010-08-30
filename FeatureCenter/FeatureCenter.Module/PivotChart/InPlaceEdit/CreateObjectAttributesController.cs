using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.PivotChart.InPlaceEdit
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        private const string InPlaceEdit_DetailView = "InPlaceEdit_DetailView";
        protected override string GetTypeToDecorate() {
            return typeof(Analysis).FullName;
        }
        protected override CloneViewAttribute GetCloneViewAttribute()
        {
            return new CloneViewAttribute(CloneViewType.DetailView, InPlaceEdit_DetailView);
        }
        protected override NavigationItemAttribute GetNavigationItemAttribute()
        {
            return new NavigationItemAttribute("PivotChart/In Place Edit", InPlaceEdit_DetailView) { ObjectKey = "Name='InPlaceEdit'" };
        }
        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute(InPlaceEdit_DetailView, new BinaryOperator("Name", "InPlaceEdit"));
        }
    }
}
