namespace eXpand.ExpressApp.PivotChart.Web {
    public class PivotedPropertyController : PivotChart.PivotedPropertyController
    {
        protected override void AttachControllers(System.Collections.Generic.IEnumerable<DevExpress.ExpressApp.DC.IMemberInfo> memberInfos)
        {
            base.AttachControllers(memberInfos);
            Frame.RegisterController(new AnalysisControlVisibilityController { TargetObjectType = View.ObjectTypeInfo.Type });
            Frame.RegisterController(new PivotGridInplaceEditorsController { TargetObjectType = View.ObjectTypeInfo.Type });
        }
    }
}