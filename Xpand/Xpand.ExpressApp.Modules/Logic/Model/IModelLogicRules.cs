using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeGenerators;


namespace Xpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    public interface IModelLogicRules : IModelNode, IModelList<IModelLogicRule> {
    }
}