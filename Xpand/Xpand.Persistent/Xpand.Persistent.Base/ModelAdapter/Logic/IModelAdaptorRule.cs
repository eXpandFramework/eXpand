using System;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.Persistent.Base.ModelAdapter.Logic {
    public interface IModelAdaptorRule : ILogicRule {
        Type RuleType { get; set; }
    }

    public interface IContextModelAdaptorRule:IContextLogicRule,IModelAdaptorRule {
         
    }
}
