namespace eXpand.ExpressApp.PivotChart.Win
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
            AttachController<PivotGridInplaceEditorsController>();
        }
    }
}
