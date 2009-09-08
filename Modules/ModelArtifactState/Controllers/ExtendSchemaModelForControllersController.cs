using eXpand.ExpressApp.ModelArtifactState.NodeWrappers;

namespace eXpand.ExpressApp.ModelArtifactState.Controllers
{
    public partial class ExtendSchemaModelForControllersController : ExtendSchemaModelController
    {
        public const string ConditionalControlllerState = "ConditionalControlllerState12";
        public ExtendSchemaModelForControllersController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string GetElementStateNodeName()
        {
            return ControllerStateRuleNodeWrapper.NodeNameAttribute;
        }

        public override string GetMoreSchema()
        {
            return @"<Attribute Name=""" + ControllerStateRuleNodeWrapper.ControllerTypeAttribute +
                   @""" IsLocalized=""False"" RefNodeName=""/Application/ActionDesign/Controllers/*"" />";
        }

        public override string GetElementStateGroupNodeName()
        {
            return ConditionalControllerStateRuleNodeWrapper.NodeNameAttribute;
        }
    }
}