using eXpand.ExpressApp.ModelArtifactState.NodeWrappers;

namespace eXpand.ExpressApp.ModelArtifactState.Controllers
{
    public partial class ExtendSchemaModelForActionsController : ExtendSchemaModelController
    {
        
        public ExtendSchemaModelForActionsController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string GetElementStateNodeName()
        {
            return ActionStateRuleNodeWrapper.NodeNameAttribute;
        }

        public override string GetMoreSchema()
        {
            return @"<Attribute Name=""" + ActionStateRuleNodeWrapper.ActionIdAttribute +
                   @""" IsLocalized=""False"" RefNodeName=""/Application/ActionDesign/Actions/Action"" />";
        }

        public override string GetElementStateGroupNodeName()
        {
            return ConditionalActionStateRuleNodeWrapper.NodeNameAttribute;
        }
    }
}