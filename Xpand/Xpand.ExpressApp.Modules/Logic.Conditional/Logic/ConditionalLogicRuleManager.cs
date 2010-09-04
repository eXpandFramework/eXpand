using System;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;

namespace Xpand.ExpressApp.Logic.Conditional.Logic {
    public class ConditionalLogicRuleManager<TConditionalLogicRule> : LogicRuleManager<TConditionalLogicRule> where TConditionalLogicRule : IConditionalLogicRule
    {
        /// <summary>
        /// Determines whether a passed object satisfies to the target criteria and the editor's customization according to a given business criteria should be performed.
        /// </summary>
        public static bool Fit(object targetObject, IConditionalLogicRule logicRule)
        {
            string criteria = logicRule.NormalCriteria;
            return targetObject == null
                       ? string.IsNullOrEmpty(logicRule.EmptyCriteria) || Fit(new object(), logicRule.EmptyCriteria)
                       : Fit(targetObject, criteria);
        }

        static bool Fit(object targetObject, string criteria)
        {
            Type objectType = targetObject.GetType();
            var wrapper = new LocalizedCriteriaWrapper(objectType, criteria);
            wrapper.UpdateParametersValues(targetObject);
            ObjectSpace objectSpace = ObjectSpace.FindObjectSpace(targetObject);
            EvaluatorContextDescriptor descriptor = objectSpace != null ? objectSpace.GetEvaluatorContextDescriptor(objectType) : new EvaluatorContextDescriptorDefault(objectType);
            var evaluator = new ExpressionEvaluator(descriptor, wrapper.CriteriaOperator);
            return evaluator.Fit(targetObject);
        }

        public static void CalculateLogicRuleInfo(LogicRuleInfo<TConditionalLogicRule> calculateLogicRuleInfo)
        {
            calculateLogicRuleInfo.Active = Fit(calculateLogicRuleInfo.Object, calculateLogicRuleInfo.Rule);
        }
    }
}