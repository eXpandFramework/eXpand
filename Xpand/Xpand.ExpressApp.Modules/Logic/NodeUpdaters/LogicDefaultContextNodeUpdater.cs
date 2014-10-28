using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Logic.DomainLogic;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.NodeUpdaters {
    public class LogicDefaultContextNodeUpdater : ModelNodesGeneratorUpdater<ExecutionContextNodeGenerator>  {
        readonly List<ExecutionContext> _executionContexts;
        readonly Func<IModelApplication, IModelLogicExecutionContextWrapper> _modelLogic;

        public LogicDefaultContextNodeUpdater(IEnumerable<ExecutionContext> executionContexts, Func<IModelApplication, IModelLogicExecutionContextWrapper> modelLogic) {
            _executionContexts = executionContexts.Concat(new[]{ExecutionContext.CurrentObjectChanged}).ToList();
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
            return _modelLogic.Invoke(application).ExecutionContextsGroup.SingleOrDefault(context => context.Id == ContextLogicRuleDomainLogic.DefaultExecutionContextGroup);
        }

        IEnumerable<ExecutionContext> GetContexts(IModelExecutionContexts modelExecutionContexts) {
            return _executionContexts.Where(executionContext => modelExecutionContexts[executionContext.ToString()] == null);
        }
    }
}