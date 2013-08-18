using System;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.Persistent.Base.Logic.Model {
    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    [Obsolete]
    public interface IModelLogicRules : IModelNode, IModelList<IModelLogicRule> {
    }
}