using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState {
    public class ActionStateModule :
        ModelRuleProviderModuleBase<ActionStateRuleAttribute, ActionStateRulesNodeWrapper,
            ActionStateRuleNodeWrapper, ActionStateRuleInfo, ActionStateRule,ActionStateRulePermission>
    {
        public override string ModelRulesNodeAttributeName {
            get { return ActionStateRulesNodeWrapper.NodeNameAttribute; }
        }

        public override DictionaryNode GetRootNode(Dictionary dictionary)
        {
            return dictionary.RootNode.GetChildNode(ModelArtifactStateModule.ModelArtifactStateAttributeName);
        }
        public override Schema GetSchema() {
            var schema = base.GetSchema();
            var dictionaryNode = schema.RootNode.GetChildNode("Element", "Name", ModelArtifactStateModule.ModelArtifactStateAttributeName);
            var findChildNode = schema.RootNode.FindChildNode("Element","Name",ModelRulesNodeAttributeName);
            schema.RootNode.RemoveChildNode(findChildNode);
            dictionaryNode.AddChildNode(findChildNode);
            return schema;
        }

        protected override string GetMoreSchema()
        {
            return new SchemaHelper().Serialize<IActionStateRule>(true);
        }

        public override string GetElementNodeName()
        {
            return ActionStateRuleNodeWrapper.NodeNameAttribute;
        }
    }
}