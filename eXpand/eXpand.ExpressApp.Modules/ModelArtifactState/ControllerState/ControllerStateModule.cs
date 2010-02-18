using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ControllerState {
    public class ControllerStateModule :
        ModelRuleProviderModuleBase<ControllerStateRuleAttribute, ControllerStateRulesNodeWrapper,
            ControllerStateRuleNodeWrapper, ControllerStateRuleInfo, ControllerStateRule,ControllerStateRulePermission> {
        public override string ModelRulesNodeAttributeName {
            get { return ControllerStateRulesNodeWrapper.NodeNameAttribute; }
        }

        public override DictionaryNode GetRootNode(Dictionary dictionary) {
            return dictionary.RootNode.GetChildNode(ModelArtifactStateModule.ModelArtifactStateAttributeName);
        }

        public override Schema GetSchema() {
            Schema schema = base.GetSchema();
            DictionaryNode dictionaryNode = schema.RootNode.GetChildNode("Element", "Name",
                                                                         ModelArtifactStateModule.ModelArtifactStateAttributeName);
            DictionaryNode findChildNode = schema.RootNode.FindChildNode("Element", "Name", ModelRulesNodeAttributeName);
            schema.RootNode.RemoveChildNode(findChildNode);
            dictionaryNode.AddChildNode(findChildNode);
            return schema;
        }

        protected override string GetMoreSchema() {
            return new SchemaHelper().Serialize<IControllerStateRule>(true);
        }

        public override string GetElementNodeName() {
            return ControllerStateRuleNodeWrapper.NodeNameAttribute;
        }
    }
}