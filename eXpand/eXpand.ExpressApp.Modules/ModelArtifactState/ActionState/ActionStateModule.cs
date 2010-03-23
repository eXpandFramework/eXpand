using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState {
    public class ActionStateModule : ArtifactStateModule<IActionStateRule>
    {
        public override string LogicRulesNodeAttributeName {
            get { return ActionStateRulesNodeWrapper.NodeNameAttribute; }
        }


        public override string RootNodePath {
            get { return ModelArtifactStateModule.ModelArtifactStateAttributeName; }
        }
        protected override void ModifySchemaAttributes(Core.DictionaryHelpers.AttibuteCreatedEventArgs args)
        {
            base.ModifySchemaAttributes(args);
            if (args.Attribute.IndexOf("ActionId") > -1)
                args.AddTag(@"Required=""True"" RefNodeName=""/Application/ActionDesign/Actions/*""");
        }

        protected override bool IsDefaultContext(ExecutionContext context) {
            return true;
        }

        public override Schema GetSchema() {
            var schema = base.GetSchema();
            var dictionaryNode = schema.RootNode.GetChildNode("Element", "Name", ModelArtifactStateModule.ModelArtifactStateAttributeName);
            var findChildNode = schema.RootNode.FindChildNode("Element","Name",LogicRulesNodeAttributeName);
            schema.RootNode.RemoveChildNode(findChildNode);
            dictionaryNode.AddChildNode(findChildNode);
            return schema;
        }


        public override string GetElementNodeName()
        {
            return ActionStateRuleNodeWrapper.NodeNameAttribute;
        }
    }
}