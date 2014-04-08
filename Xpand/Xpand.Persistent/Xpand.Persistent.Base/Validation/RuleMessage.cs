using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Validation {
    public class RuleMessage : RuleBase {
        public RuleMessage(ContextIdentifiers targetContextIDs, Type targetType)
            : base("", targetContextIDs, targetType) {
        }

        protected override bool IsValidInternal(
           object target, out string errorMessageTemplate) {
            errorMessageTemplate = Properties.CustomMessageTemplate;

            return false;
        }
    }

    public static class Extensions {
        public static RuleSetValidationResult NewRuleSetValidationMessageResult(this RuleSet ruleSet, IObjectSpace objectSpace, string messageTemplate, ContextIdentifier contextIdentifier, object objectTarget, Type targeObjecttType) {
            var rule = new RuleMessage(contextIdentifier, targeObjecttType);
            rule.Properties.SkipNullOrEmptyValues = false;
            rule.Properties.CustomMessageTemplate = messageTemplate;
            Validator.RuleSet.RegisteredRules.Add(rule);
            RuleSetValidationResult validationResult;
            using (objectSpace.CreateParseCriteriaScope()) {
                validationResult = Validator.RuleSet.ValidateTarget(objectSpace, objectTarget, contextIdentifier);
            }
            Validator.RuleSet.RegisteredRules.Remove(rule);
            return validationResult;
        }
    }
}
