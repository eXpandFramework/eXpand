using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.XtraPivotGrid;
using XVideoRental.Module.Win.BusinessObjects;

namespace XVideoRental.Module.Win.Controllers {
    public class MediaPerformance_Movie_SummuryController : ViewController<ListView> {
        public MediaPerformance_Movie_SummuryController() {
            TargetViewId = ViewIdProvider.MediaPerformance_Movie_Summury;
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            ((PivotGridListEditor)View.Editor).PivotGridControl.FieldValueDisplayText += PivotGridControlOnFieldValueDisplayText;
        }

        void PivotGridControlOnFieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e) {
            if (e.Field != null && e.Field.FieldName == "Item.Movie") {
                IMemberInfo member = View.Model.ModelClass.TypeInfo.FindMember("Item.Movie");
                IMemberInfo memberInfo = member.MemberTypeInfo.FindMember("Title");
                e.DisplayText = memberInfo.GetValue(e.Value) as string;
            }
        }
    }
}
