using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.NodeUpdaters {
    public class LogicDefaultContextNodeUpdater : ModelNodesGeneratorUpdater<ExecutionContextNodeGenerator>  {
        readonly List<ExecutionContext> _executionContexts;
        readonly Func<IModelApplication, IModelLogic> _modelLogic;

        public LogicDefaultContextNodeUpdater(List<ExecutionContext> executionContexts, Func<IModelApplication, IModelLogic> modelLogic) {
            _executionContexts = executionContexts;
            _modelLogic = modelLogic;
        }


        public override void UpdateNode(ModelNode node) {
            IModelExecutionContexts defaultModelExecutionContexts = GetDefaulModelExecutionContextsModelNode(node.Application);
            if (defaultModelExecutionContexts != null) {
                foreach (var executionContext in GetContexts(defaultModelExecutionContexts)) {
                    var modelExecutionContext = defaultModelExecutionContexts.AddNode<IModelExecutionContext>();
                    modelExecutionContext.Name = executionContext.ToString();
                }
            }
        }

        IModelExecutionContexts GetDefaulModelExecutionContextsModelNode(IModelApplication application) {
            return _modelLogic.Invoke(application).ExecutionContextsGroup.SingleOrDefault(context => context.Id == LogicRuleDomainLogic.DefaultExecutionContextGroup);
        }

        IEnumerable<ExecutionContext> GetContexts(IModelExecutionContexts modelExecutionContexts) {
            return _executionContexts.Where(executionContext => modelExecutionContexts[executionContext.ToString()] == null);
        }
    }
}