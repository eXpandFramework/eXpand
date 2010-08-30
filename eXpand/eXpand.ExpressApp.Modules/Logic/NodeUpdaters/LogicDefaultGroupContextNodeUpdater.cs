using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeGenerators;

namespace eXpand.ExpressApp.Logic.NodeUpdaters {
    public abstract class LogicDefaultGroupContextNodeUpdater : ModelNodesGeneratorUpdater<GroupContextsNodeGenerator> {
        public const string Default = "Default";

        public override void UpdateNode(ModelNode node) {
            IModelGroupContexts m = GetModelLogicNode(node).GroupContexts;
            if (m.GetNode<IModelExecutionContexts>(Default) != null) return;
            m.AddNode<IModelExecutionContexts>(Default);
        }

        protected abstract IModelLogic GetModelLogicNode(ModelNode node);
    }
}