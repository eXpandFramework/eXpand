using System;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Xpand.ExpressApp.Validation.RuleType {
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

        protected RuleType GetRuleType(IRuleBaseProperties properties) {
            var resultType = properties.ResultType;
            switch (resultType) {
                case ValidationResultType.Error:
                    return RuleType.Critical;

                case ValidationResultType.Information:
                    return RuleType.Information;

                case ValidationResultType.Warning:
                    return RuleType.Warning;
            }

            throw new InvalidOperationException("Unknown ResultType");
        }
    }
}
