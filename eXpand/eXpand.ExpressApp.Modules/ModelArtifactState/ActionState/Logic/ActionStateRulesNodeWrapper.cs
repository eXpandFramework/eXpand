using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public class ActionStateRulesNodeWrapper : LogicRulesNodeWrapper<IActionStateRule>
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
            IActionStateRule wrapper = base.AddRule(logicRuleAttribute, typeInfo, logicRuleNodeWrapper);
            var actionStateRuleAttribute = logicRuleAttribute;
            wrapper.ActionId = actionStateRuleAttribute.ActionId;
            wrapper.Module = actionStateRuleAttribute.Module;
            wrapper.ActionState =  actionStateRuleAttribute.ActionState;
            return wrapper;
        }
    }
}