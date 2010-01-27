namespace eXpand.ExpressApp.PivotChart.Win.Controllers
{
    public partial class PivotedPropertyController : PivotChart.PivotedPropertyController
    {
        public PivotedPropertyController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override void AttachControllers(DevExpress.ExpressApp.DC.IMemberInfo memberInfo)
        {
            base.AttachControllers(memberInfo);
            var pivotGridInplaceEditorsController = new PivotGridInplaceEditorsController {TargetObjectType = View.ObjectTypeInfo.Type};
            Frame.RegisterController(pivotGridInplaceEditorsController);
        }
    }
}


