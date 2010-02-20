using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic.Conditional;
using eXpand.ExpressApp.ModelArtifactState.ActionState.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState {
    public class ActionStateModule :
        ConditionalLogicRuleProviderModuleBase<IActionStateRule>
    {
        public override string LogicRulesNodeAttributeName {
            get { return ActionStateRulesNodeWrapper.NodeNameAttribute; }
        }

        public override DictionaryNode GetRootNode(Dictionary dictionary)
        {
            return dictionary.RootNode.GetChildNode(ModelArtifactStateModule.ModelArtifactStateAttributeName);
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