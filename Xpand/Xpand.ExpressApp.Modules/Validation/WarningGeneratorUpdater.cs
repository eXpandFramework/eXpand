using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Validation;

namespace Xpand.ExpressApp.Validation {
    public class WarningGeneratorUpdater : ModelNodesGeneratorUpdater<ModelValidationRulesNodeGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelValidationRules = ((IModelValidationRules)node).OfType<IRuleBaseProperties>();
            foreach (var validationRule in modelValidationRules) {
                var modelRuleBaseWarning = ((IModelRuleBaseWarning)validationRule);
                modelRuleBaseWarning.IsWarning = IsWarning(validationRule);
            }
        }

        bool IsWarning(IRuleBaseProperties validationRule) {
            var modelClass = ((IModelNode)validationRule).Application.BOModel[validationRule.TargetType.FullName];
            if (modelClass != null) {
                var isClassWaring = modelClass.TypeInfo.FindAttributes<RuleWarningAttribute>().FirstOrDefault(attribute => attribute.Id == validationRule.Id) != null;
                return isClassWaring || IsMemberWarning(validationRule as IRulePropertyValueProperties, modelClass);
            }
            return false;
        }


        bool IsMemberWarning(IRulePropertyValueProperties validationRule, IModelClass modelClass) {
            if (validationRule != null) {
                var modelMember = modelClass.FindMember(validationRule.TargetPropertyName);
                return modelMember != null && modelMember.MemberInfo.FindAttributes<RuleWarningAttribute>().FirstOrDefault(attribute => attribute.Id == validationRule.Id) != null;
            }
            return false;
        }
    }
}
