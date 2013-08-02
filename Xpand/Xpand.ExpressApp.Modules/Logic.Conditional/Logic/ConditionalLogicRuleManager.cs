using DevExpress.Data.Filtering;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic.Conditional.Logic {
    public class ConditionalLogicRuleManager<TConditionalLogicRule> : LogicRuleManager<TConditionalLogicRule> where TConditionalLogicRule : IConditionalLogicRule {
        /// <summary>
        /// Determines whether a passed object satisfies to the target criteria and the editor's customization according to a given business criteria should be performed.
        /// </summary>
        public static bool Fit(object targetObject, IConditionalLogicRule logicRule) {
            var criteria = CriteriaOperator.Parse(logicRule.NormalCriteria);
            return targetObject == null
                       ? string.IsNullOrEmpty(logicRule.EmptyCriteria) || CriteriaOperator.Parse(logicRule.EmptyCriteria).Fit(new object())
                       : criteria.Fit(targetObject);
        }


        public static void CalculateLogicRuleInfo(LogicRuleInfo<TConditionalLogicRule> calculateLogicRuleInfo) {
            calculateLogicRuleInfo.Active = Fit(calculateLogicRuleInfo.Object, calculateLogicRuleInfo.Rule);
        }
    }
}