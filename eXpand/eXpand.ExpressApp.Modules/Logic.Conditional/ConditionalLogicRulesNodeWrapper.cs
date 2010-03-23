using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

namespace eXpand.ExpressApp.Logic.Conditional {
    public abstract class ConditionalLogicRulesNodeWrapper<TConditionalLogicRule> : LogicRulesNodeWrapper<TConditionalLogicRule> where TConditionalLogicRule:IConditionalLogicRule
    {
        protected ConditionalLogicRulesNodeWrapper(DictionaryNode dictionaryNode) : base(dictionaryNode) {
        }

        public override TConditionalLogicRule AddRule(TConditionalLogicRule logicRuleAttribute, ITypeInfo typeInfo, Type logicRuleNodeWrapper) {
            var conditionalLogicRule = base.AddRule(logicRuleAttribute, typeInfo, logicRuleNodeWrapper);
            conditionalLogicRule.EmptyCriteria = logicRuleAttribute.EmptyCriteria;
            conditionalLogicRule.NormalCriteria = logicRuleAttribute.NormalCriteria;
            return conditionalLogicRule;
        }
    }
}