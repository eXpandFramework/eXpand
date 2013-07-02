using System;
using Xpand.ExpressApp.Logic.Conditional.Logic;

namespace Xpand.ExpressApp.ModelAdaptor.Logic {
    public interface IModelAdaptorRule : IConditionalLogicRule {
        Type RuleType { get; set; }
    }
}
