using System;
using Xpand.ExpressApp.Logic.Conditional.Logic;

namespace Xpand.ExpressApp.ModelAdaptor.Logic {
    public class ModelAdaptorRule : ConditionalLogicRule, IModelAdaptorRule {
        public ModelAdaptorRule(IModelAdaptorRule modelAdaptorRule)
            : base(modelAdaptorRule) {   
            RuleType=modelAdaptorRule.RuleType;
        }

        public Type RuleType { get; set; }
    }
}
