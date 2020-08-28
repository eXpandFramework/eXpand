namespace Xpand.ExpressApp.PivotChart.Win.PivotedProperty {
    public class PivotedPropertyController : PivotChart.PivotedProperty.PivotedPropertyController {
        protected override void AttachControllers() {
            base.AttachControllers();
            Frame.RegisterController(new PivotGridInplaceEditorsController {TargetObjectType = View.ObjectTypeInfo.Type});
            Frame.RegisterController(new AnalysisControlVisibilityController{TargetObjectType = View.ObjectTypeInfo.Type});
            Frame.RegisterController(new AnalysisDisplayDateTimeViewController { TargetObjectType = View.ObjectTypeInfo.Type });
            Frame.RegisterController(new PivotCustomSortController { TargetObjectType = View.ObjectTypeInfo.Type });
        }
    }
}