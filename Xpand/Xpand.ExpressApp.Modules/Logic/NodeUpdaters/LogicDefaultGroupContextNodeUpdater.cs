using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.NodeUpdaters {
    public abstract class LogicDefaultGroupContextNodeUpdater : ModelNodesGeneratorUpdater<ExecutionContextsGroupNodeGenerator> {
        public const string Default = "Default";

        public override void UpdateNode(ModelNode node) {
            IModelExecutionContextsGroup m = GetModelLogicNode(node).ExecutionContextsGroup;
            if (m.GetNode<IModelExecutionContexts>(Default) != null) return;
            m.AddNode<IModelExecutionContexts>(Default);
        }

        protected abstract IModelLogic GetModelLogicNode(ModelNode node);
    }
}