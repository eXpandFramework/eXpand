using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public class ActionStateRulesNodeWrapper : ArtifactStateRulesNodeWrapper<IActionStateRule>
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
        public override IActionStateRule AddRule(IActionStateRule logicRuleAttribute, ITypeInfo typeInfo, Type logicRuleNodeWrapper) {
            IActionStateRule actionStateRule = base.AddRule(logicRuleAttribute, typeInfo, logicRuleNodeWrapper);
            actionStateRule.ActionId = logicRuleAttribute.ActionId;
            actionStateRule.ActionState = logicRuleAttribute.ActionState;
            return actionStateRule;
        }
    }
}