using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Xpand.ExpressApp.Validation {
    public enum RuleType {
        Critical,
        [ImageName("Warning")]
        Warning,
        [ImageName("Information")]
        Information
    }
    public class RuleTypeGeneratorUpdater : ModelNodesGeneratorUpdater<ModelValidationRulesNodeGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelValidationRules = ((IModelValidationRules)node).OfType<IRuleBaseProperties>();
            foreach (var validationRule in modelValidationRules) {
                var type = validationRule as IModelRuleBaseRuleType;
                if (type != null) {
                    type.RuleType = GetRuleType(validationRule);
                }
            }
        }

        RuleType GetRuleType(IRuleBaseProperties validationRule) {
            var modelClass = ((IModelNode)validationRule).Application.BOModel[validationRule.TargetType.FullName];
            if (modelClass != null) {
                var ruleType = GetRuleType(modelClass, validationRule);
                return ruleType != RuleType.Critical ? ruleType : GetRuleType(validationRule as IRulePropertyValueProperties, modelClass);
            }
            return RuleType.Critical;
        }

        RuleType GetRuleType(IModelClass modelClass, IRuleBaseProperties validationRule) {
            var ruleErrorTypeAttribute = modelClass.TypeInfo.FindAttributes<RuleErrorTypeAttribute>().FirstOrDefault(attribute => attribute.Id == validationRule.Id);
            return ruleErrorTypeAttribute != null ? ruleErrorTypeAttribute.RuleType : RuleType.Critical;
        }

        RuleType GetRuleType(IRulePropertyValueProperties validationRule, IModelClass modelClass) {
            if (validationRule != null) {
                var modelMember = modelClass.FindMember(validationRule.TargetPropertyName);
                if (modelMember != null) {
                    var ruleErrorTypeAttribute = modelMember.MemberInfo.FindAttributes<RuleErrorTypeAttribute>().FirstOrDefault(attribute => attribute.Id == validationRule.Id);
                    if (ruleErrorTypeAttribute != null) {
                        return ruleErrorTypeAttribute.RuleType;
                    }
                }
            }
            return RuleType.Critical;
        }
    }
}
