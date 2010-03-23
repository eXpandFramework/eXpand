using eXpand.ExpressApp.ModelArtifactState.Attributes;
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
                   @""" IsLocalized=""False"" RefNodeName=""/Application/ActionDesign/Actions/Action"" />"+
                   @"<Attribute Name=""State"" Choice=""{" + typeof(ActionState).FullName + @"}""/>";
        }

        public override string GetElementStateGroupNodeName()
        {
            return ActionStateRulesNodeWrapper.NodeNameAttribute;
        }
    }
}