using Xpand.ExpressApp.PivotChart.Web.InPlaceEdit;

namespace Xpand.ExpressApp.PivotChart.Web.PivotedProperty {
    public class PivotedPropertyController : PivotChart.PivotedProperty.PivotedPropertyController{
        protected override void AttachControllers(System.Collections.Generic.IEnumerable<DevExpress.ExpressApp.DC.IMemberInfo> memberInfos){
            base.AttachControllers(memberInfos);
            Frame.RegisterController(new PivotGridInplaceEditorsController { TargetObjectType = View.ObjectTypeInfo.Type });
            Frame.RegisterController(new AnalysisControlVisibilityController { TargetObjectType = View.ObjectTypeInfo.Type });
            Frame.RegisterController(new AnalysisDisplayDateTimeViewController { TargetObjectType = View.ObjectTypeInfo.Type });
            Frame.RegisterController(new PivotCustomSortController { TargetObjectType = View.ObjectTypeInfo.Type });
        }
    }
}