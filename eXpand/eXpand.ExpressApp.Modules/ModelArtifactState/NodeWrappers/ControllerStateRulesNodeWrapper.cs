using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.ModelArtifactState.Attributes;
using eXpand.ExpressApp.RuleModeller;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers {
    public class ControllerStateRulesNodeWrapper : ModelRulesNodeWrapper<ControllerStateRuleNodeWrapper, ControllerStateRuleAttribute>
    {
        public const string NodeNameAttribute = "ConditionalControllerState";

        public ControllerStateRulesNodeWrapper() : this(new DictionaryNode(NodeNameAttribute)) {
        }

        public ControllerStateRulesNodeWrapper(DictionaryNode conditionalartifactStateNode)
            : base(conditionalartifactStateNode) {
        }


        protected override string ChildNodeName {
            get { return ControllerStateRuleNodeWrapper.NodeNameAttribute; }
        }

        public override ControllerStateRuleNodeWrapper AddRule(
            ControllerStateRuleAttribute modelRuleAttribute, ITypeInfo typeInfo) {
            ControllerStateRuleNodeWrapper stateRuleNodeWrapper =
                base.AddRule(modelRuleAttribute, typeInfo);
            stateRuleNodeWrapper.ControllerType =modelRuleAttribute.ControllerType.FullName;
            stateRuleNodeWrapper.State = modelRuleAttribute.State;
            return stateRuleNodeWrapper;
        }
    }
}