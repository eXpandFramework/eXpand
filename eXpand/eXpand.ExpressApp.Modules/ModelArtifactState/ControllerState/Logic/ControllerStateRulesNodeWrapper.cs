using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ControllerState.Logic {
    public class ControllerStateRulesNodeWrapper : ArtifactStateRulesNodeWrapper<IControllerStateRule>
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

        public override IControllerStateRule AddRule(IControllerStateRule logicRuleAttribute, ITypeInfo typeInfo, Type logicRuleNodeWrapper) {
            IControllerStateRule stateRuleNodeWrapper =base.AddRule(logicRuleAttribute, typeInfo, logicRuleNodeWrapper);
            stateRuleNodeWrapper.ControllerType = logicRuleAttribute.ControllerType;
            stateRuleNodeWrapper.State = logicRuleAttribute.State;
            return stateRuleNodeWrapper;
        }
    }
}