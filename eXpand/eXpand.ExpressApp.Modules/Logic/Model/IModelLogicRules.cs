using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.NodeGenerators;

namespace eXpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof (LogicRulesNodesGenerator))]
    public interface IModelLogicRules : IModelNode, IModelList<IModelLogicRule> {
    }
}