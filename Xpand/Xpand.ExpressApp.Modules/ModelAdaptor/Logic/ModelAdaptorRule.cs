using System;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.ModelAdapter.Logic;

namespace Xpand.ExpressApp.ModelAdaptor.Logic {
    public class ModelAdaptorRule : LogicRule, IModelAdaptorRule {
        public ModelAdaptorRule(IModelAdaptorRule modelAdaptorRule)
            : base(modelAdaptorRule) {   
            RuleType=modelAdaptorRule.RuleType;
        }

        public Type RuleType { get; set; }
    }
}
