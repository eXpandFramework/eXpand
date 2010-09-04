namespace Xpand.ExpressApp.Logic.Conditional.Logic {
    public abstract class ConditionalLogicRuleViewController<TConditionalLogicRule> :
        LogicRuleViewController<TConditionalLogicRule> where TConditionalLogicRule : IConditionalLogicRule{
        protected override LogicRuleInfo<TConditionalLogicRule> CalculateLogicRuleInfo(object targetObject,TConditionalLogicRule logicRule) {
            LogicRuleInfo<TConditionalLogicRule> calculateLogicRuleInfo = base.CalculateLogicRuleInfo(targetObject,logicRule);
            ConditionalLogicRuleManager<TConditionalLogicRule>.CalculateLogicRuleInfo(calculateLogicRuleInfo);
            return calculateLogicRuleInfo;
        }
    }
}