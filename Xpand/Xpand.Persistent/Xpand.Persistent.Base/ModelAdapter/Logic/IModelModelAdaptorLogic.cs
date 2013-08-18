using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.Persistent.Base.ModelAdapter.Logic {
    
    public interface IModelModelAdaptorLogic : IModelLogicContexts {
        IModelModelAdaptorLogicRules Rules { get; }
    }
    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    public interface IModelModelAdaptorLogicRules : IModelNode, IModelList<IModelModelAdaptorRule> {
    }

    public interface IModelAdaptorLogicIntaller:ILogicInstaller {
         
    }
}
