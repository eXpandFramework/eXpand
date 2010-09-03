using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.PivotChart.ControllingPivotGridSettings
{
    public class CreateObjectAttributesController : Module.CreateObjectAttributesController
    {
        private const string ControllingPivotGridSettings_DetailView = "ControllingPivotGridSettings_DetailView";
        protected override string GetTypeToDecorate() {
            return typeof(Analysis).FullName;
        }
        protected override CloneViewAttribute GetCloneViewAttribute()
        {
            return new CloneViewAttribute(CloneViewType.DetailView, ControllingPivotGridSettings_DetailView);
        }
        protected override XpandNavigationItemAttribute GetNavigationItemAttribute()
        {
            return new XpandNavigationItemAttribute("PivotChart/Controlling Grid Settings", ControllingPivotGridSettings_DetailView) { ObjectKey = "Name='Controlling Grid Settings'" };
        }
        protected override DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute() {
            return new DisplayFeatureModelAttribute(ControllingPivotGridSettings_DetailView, new BinaryOperator("Name", "Controlling Grid Settings"));
        }
    }
}
