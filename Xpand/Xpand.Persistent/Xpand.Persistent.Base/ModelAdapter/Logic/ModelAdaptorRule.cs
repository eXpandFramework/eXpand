using System;
using Xpand.Persistent.Base.Logic;

namespace Xpand.Persistent.Base.ModelAdapter.Logic {
    public class ModelAdaptorRule : LogicRule, IModelAdaptorRule {
        public ModelAdaptorRule(IModelAdaptorRule modelAdaptorRule)
            : base(modelAdaptorRule) {   
            RuleType=modelAdaptorRule.RuleType;
        }

        public Type RuleType { get; set; }
    }
}
