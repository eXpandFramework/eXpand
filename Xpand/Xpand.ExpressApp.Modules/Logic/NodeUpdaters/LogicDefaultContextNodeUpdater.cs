using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.NodeUpdaters {
    public abstract class LogicDefaultContextNodeUpdater : ModelNodesGeneratorUpdater<ExecutionContextNodeGenerator> {
        public override void UpdateNode(ModelNode node) {
            IModelExecutionContexts defaultModelExecutionContexts = GetDefaulModelExecutionContextsModelNode(node);
            if (defaultModelExecutionContexts != null) {
                foreach (ExecutionContext executionContext in GetContexts(defaultModelExecutionContexts)) {
                    var modelExecutionContext = defaultModelExecutionContexts.AddNode<IModelExecutionContext>();
                    modelExecutionContext.Name=executionContext.ToString();
                }
            }
        }

        IModelExecutionContexts GetDefaulModelExecutionContextsModelNode(ModelNode node)
        {
            return GetModelLogicNode(node).ExecutionContextsGroup.Where(
                context => context.Id == LogicDefaultGroupContextNodeUpdater.Default).SingleOrDefault();
        }


        IEnumerable<ExecutionContext> GetContexts(IModelExecutionContexts modelExecutionContexts)
        {
            List<ExecutionContext> executionContexts = GetExecutionContexts();
            return executionContexts.Where(executionContext => modelExecutionContexts.GetNode<IModelExecutionContext>(executionContext.ToString()) == null);
        }

        protected abstract List<ExecutionContext> GetExecutionContexts();

        protected abstract IModelLogic GetModelLogicNode(ModelNode node);
    }
}