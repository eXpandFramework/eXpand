using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeGenerators;

namespace eXpand.ExpressApp.Logic.NodeUpdaters {
    public abstract class LogicDefaultContextNodeUpdater : ModelNodesGeneratorUpdater<ExecutionContextNodeGenerator> {
        public override void UpdateNode(ModelNode node) {
            IModelNode modelNode =GetModelLogicNode(node).GroupContexts.Where(
                    context => context.Id == LogicDefaultGroupContextNodeUpdater.Default).SingleOrDefault();
            if (modelNode != null) {
                List<ExecutionContext> executionContexts = GetExecutionContexts();
                var contexts = executionContexts.Where(executionContext => modelNode.GetNode<IModelExecutionContext>(executionContext.ToString()) == null);
                foreach (ExecutionContext executionContext in contexts) {
                    modelNode.AddNode<IModelExecutionContext>(executionContext.ToString());
                }
            }
        }

        protected abstract List<ExecutionContext> GetExecutionContexts();

        protected abstract IModelLogic GetModelLogicNode(ModelNode node);
    }
}