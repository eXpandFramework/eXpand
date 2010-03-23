namespace eXpand.ExpressApp.Logic.Conditional {
    public abstract class ConditionalLogicRuleViewController<TConditionalLogicRule> : LogicRuleViewController<TConditionalLogicRule> where TConditionalLogicRule : IConditionalLogicRule
    {
        protected override LogicRuleInfo<TConditionalLogicRule> CalculateLogicRuleInfo(object targetObject, TConditionalLogicRule logicRule)
        {
            var calculateLogicRuleInfo = base.CalculateLogicRuleInfo(targetObject, logicRule);
            ConditionalLogicRuleManager<TConditionalLogicRule>.CalculateLogicRuleInfo(calculateLogicRuleInfo);
            return calculateLogicRuleInfo;
        }

    }
}