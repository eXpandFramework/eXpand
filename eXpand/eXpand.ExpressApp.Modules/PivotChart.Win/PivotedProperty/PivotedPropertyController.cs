using System.Collections.Generic;
using DevExpress.ExpressApp.DC;

namespace eXpand.ExpressApp.PivotChart.Win.PivotedProperty {
    public class PivotedPropertyController : PivotChart.PivotedProperty.PivotedPropertyController {
        protected override void AttachControllers(IEnumerable<IMemberInfo> memberInfos) {
            base.AttachControllers(memberInfos);
            Frame.RegisterController(new PivotGridInplaceEditorsController {TargetObjectType = View.ObjectTypeInfo.Type});
            Frame.RegisterController(new AnalysisControlVisibilityController{TargetObjectType = View.ObjectTypeInfo.Type});
            Frame.RegisterController(new AnalysisDisplayDateTimeViewController { TargetObjectType = View.ObjectTypeInfo.Type });
        }
    }
}