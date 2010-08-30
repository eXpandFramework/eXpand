using eXpand.ExpressApp.ModelArtifactState.ControllerState;
using eXpand.ExpressApp.ModelArtifactState.NodeWrappers;
using eXpand.Persistent.Base.General;

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
                   @""" IsLocalized=""False"" RefNodeName=""/Application/ActionDesign/Controllers/*"" />"+
                   @"<Attribute Name=""State"" Choice=""{" + typeof(State).FullName + @"}""/>";
        }

        public override string GetElementStateGroupNodeName()
        {
            return ControllerStateRulesNodeWrapper.NodeNameAttribute;
        }
    }
}