using System;

namespace Xpand.Persistent.Base.ModelAdapter.Logic {
    public interface IModelAdaptorRuleController {
        void ExecuteLogic(Type modelAdaptorRuleType, Type modelModelAdaptorRuleType, Action<IModelAdaptorRule> action);
    }
}