using System;
using System.Linq;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Validation;

namespace Xpand.Test{    public static class ValidationAssertions {
        public static ValidationState StateOf<TRule>(this RuleSet ruleSet,object targetObject, Func<string> id)
            where TRule : RuleBase{
            return ruleSet.StateOf(targetObject, item => item.Rule is TRule && item.Rule.Id == id());
        }

        public static ValidationState StateOf<TRule>(this RuleSet ruleSet,object targetObject,string usedProperties) where TRule : RuleBase{
            return ruleSet.StateOf(targetObject,
                item => item.Rule.UsedProperties.Contains(usedProperties) && item.Rule is TRule);
        }

        public static ValidationState StateOf(this RuleSet ruleSet,object targetObject, string message) {
            return ruleSet.StateOf(targetObject,
                item =>  item.Rule.Properties.CustomMessageTemplate == message);
        }

        public static ValidationState StateOf(this RuleSet ruleSet,object targetObject,
            Func<RuleSetValidationResultItem, bool> filter){
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(targetObject);
            var ruleSetValidationResult = Validator.RuleSet.ValidateTarget(objectSpace, targetObject, ContextIdentifier.Save);
            var validationResultItem = ruleSetValidationResult.Results.First(filter);
            return validationResultItem.State;
        }
    }
}