using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Validation {
    public class RuleMessage : RuleBase {
        private readonly List<string> _usedProperties;

        public RuleMessage(ContextIdentifiers targetContextIDs, Type targetType,List<string> usedProperties=null)
            : base("", targetContextIDs, targetType){
            usedProperties = usedProperties ?? new List<string>();
            _usedProperties=usedProperties;
        }

        public override ReadOnlyCollection<string> UsedProperties => _usedProperties.AsReadOnly();

        protected override bool IsValidInternal(
           object target, out string errorMessageTemplate) {
            errorMessageTemplate = Properties.CustomMessageTemplate;

            return false;
        }
    }

    public static class Extensions {
        public static RuleSetValidationResult NewRuleSetValidationMessageResult(this RuleSet ruleSet,
            IObjectSpace objectSpace, string messageTemplate, object objectTarget){
            return ruleSet.NewRuleSetValidationMessageResult(objectSpace, messageTemplate, ContextIdentifier.Save,objectTarget, objectTarget.GetType());
        }

        public static RuleSetValidationResult NewRuleSetValidationMessageResult(this IRuleSet ruleSet,
            IObjectSpace objectSpace, string messageTemplate, ContextIdentifier contextIdentifier, object objectTarget,
            Type targeObjecttType, List<string> usedProperties = null,
            ValidationResultType resultType = ValidationResultType.Error){

            usedProperties = usedProperties ?? new List<string>();
            var rule = new RuleMessage(contextIdentifier, targeObjecttType,usedProperties);
            rule.Properties.ResultType = resultType;
            rule.Properties.SkipNullOrEmptyValues = false;
            rule.Properties.CustomMessageTemplate = messageTemplate;
            ruleSet.RegisteredRules.Add(rule);

            RuleSetValidationResult validationResult;
            using (objectSpace.CreateParseCriteriaScope()) {
                validationResult = ruleSet.ValidateTarget(objectSpace, objectTarget, contextIdentifier);
            }
            ruleSet.RegisteredRules.Remove(rule);
            return validationResult;
        }
    }
}
