using System;
using System.Text;
using DevExpress.Persistent.Validation;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using eXpand.Utils.Linq.Dynamic;

namespace eXpand.Persistent.BaseImpl.Validation.ConditionalRequired
{
    public class RuleConditionalRequired : RuleBase
    {
        protected override bool IsValidInternal(object target, out string errorMessageTemplate)
        {
            // Load the Property for with this ID
            var conditionalRules = from rule in target.GetType().GetProperties()
                                   from custAtt in rule.GetCustomAttributes(true)
                                   where custAtt.GetType() == typeof(RuleConditionalIsRequiredAttribute)
                                         && ((RuleConditionalIsRequiredAttribute)custAtt).Name == Id
                                   select rule;

            var errorMessage = new StringBuilder();

            PropertyInfo propInfo = null;
            if (conditionalRules.Count() > 0)
                propInfo = conditionalRules.First();

            if (propInfo != null)
                foreach (RuleConditionalIsRequiredAttribute customAttr in propInfo.GetCustomAttributes(typeof(RuleConditionalIsRequiredAttribute), true))
                {
                    if (CheckCondition(target, customAttr.Condition, propInfo))
                    {
                        if (propInfo.GetValue(target, null) == null || String.IsNullOrEmpty(propInfo.GetValue(target, null) as String))
                        {
                            errorMessage.AppendLine(customAttr.CustomErrorMessage);
                        }
                    }
                    
                }

            errorMessageTemplate = errorMessage.ToString();
            return errorMessage.Length == 0;
        }

        protected internal static bool CheckCondition<B>(B businessObject, String condition, PropertyInfo pInfo)
        {
            try
            {
                LambdaExpression lambdaE = DynamicExpression.ParseLambda(businessObject.GetType(), typeof(bool), condition);
                return (bool)lambdaE.Compile().DynamicInvoke(businessObject);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(String.Format("Condition: [{0}] failed on Property {1} in Business Object {2}  - Error Message: {3}",
                                                          condition, pInfo.Name, businessObject.GetType().Name, ex.Message));
            }
        }
    }
}