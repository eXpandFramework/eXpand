using System;
using Xpand.Persistent.Base.Logic;

namespace Xpand.Persistent.Base.ModelAdapter.Logic {
    public interface IModelAdaptorRule : ILogicRule {
        Type RuleType { get; set; }
    }
}
