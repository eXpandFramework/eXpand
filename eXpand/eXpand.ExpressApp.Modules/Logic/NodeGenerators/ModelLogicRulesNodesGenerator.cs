using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.Logic.NodeGenerators {
    public abstract class ModelLogicRulesNodesGenerator<TRootModelNode, TModelLogicRule> : ModelNodesGeneratorUpdater<LogicRulesNodesGenerator>
        where TRootModelNode : IModelNode
        where TModelLogicRule : IModelLogicRule
    {
        public override void UpdateNode(ModelNode node)
        {
            if (node.Parent.Parent is TRootModelNode)
                UpdateNode(((IModelLogicRules)node).OfType<TModelLogicRule>());
        }

        public abstract void UpdateNode(IEnumerable<TModelLogicRule> rules);
    }
}