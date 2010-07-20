using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeGenerators;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.Logic.NodeUpdaters {
    public abstract class LogicDefaultContextNodeUpdater : ModelNodesGeneratorUpdater<ExecutionContextNodeGenerator> {
        public override void UpdateNode(ModelNode node) {
            IModelExecutionContexts defaultModelExecutionContexts = GetDefaulModelExecutionContextsModelNode(node);
            if (defaultModelExecutionContexts != null) {
                IEnumerable<ITypeInfo> implementors = XafTypesInfo.Instance.FindTypeInfo(typeof(IModelExecutionContext)).Implementors;
                foreach (ExecutionContext executionContext in GetContexts(defaultModelExecutionContexts)) {
                    AddNode(defaultModelExecutionContexts, executionContext, implementors, executionContext);
                }
            }
        }

        IModelExecutionContexts GetDefaulModelExecutionContextsModelNode(ModelNode node)
        {
            return GetModelLogicNode(node).GroupContexts.Where(
                context => context.Id == LogicDefaultGroupContextNodeUpdater.Default).SingleOrDefault();
        }

        void AddNode(IModelNode modelNode, ExecutionContext executionContext, IEnumerable<ITypeInfo> implementors, ExecutionContext context) {
            ITypeInfo typeInfo = implementors.Where(info => info.Type.Name=="IModel"+context).First();
            modelNode.AddNode(typeInfo.Type, executionContext.ToString());
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