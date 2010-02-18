using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState {
    public class ActionStateRulesNodeWrapper : ModelRulesNodeWrapper<ActionStateRuleNodeWrapper, ActionStateRuleAttribute>
    {
        public const string NodeNameAttribute = "ConditionalActionState";

        public ActionStateRulesNodeWrapper() : this(new DictionaryNode(NodeNameAttribute)) {
        }

        public ActionStateRulesNodeWrapper(DictionaryNode conditionalartifactStateNode)
            : base(conditionalartifactStateNode) {
        }


        protected override string ChildNodeName {
            get { return ActionStateRuleNodeWrapper.NodeNameAttribute; }
        }


        public override ActionStateRuleNodeWrapper AddRule(
            ActionStateRuleAttribute stateRuleAttribute, ITypeInfo typeInfo) {
            ActionStateRuleNodeWrapper wrapper = base.AddRule(stateRuleAttribute, typeInfo);
            var actionStateRuleAttribute = stateRuleAttribute;
            wrapper.ActionId = actionStateRuleAttribute.ActionId;
            wrapper.Module = actionStateRuleAttribute.Module;
            wrapper.ActionState =  actionStateRuleAttribute.ActionState;
            return wrapper;
        }
    }
}