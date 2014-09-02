using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Logic.DomainLogic;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;
using System.Linq;

namespace Xpand.ExpressApp.Logic.NodeUpdaters {
    public class LogicDefaultGroupContextNodeUpdater : ModelNodesGeneratorUpdater<ExecutionContextsGroupNodeGenerator> {
        readonly Func<IModelApplication, IModelLogicExecutionContextWrapper> _modelLogic;


        public LogicDefaultGroupContextNodeUpdater(
            Func<IModelApplication, IModelLogicExecutionContextWrapper> modelLogic) {
            _modelLogic = modelLogic;
        }

        public override void UpdateNode(ModelNode node) {
            var contextsGroup = _modelLogic.Invoke(node.Application).ExecutionContextsGroup;
            if (contextsGroup.All(contexts => contexts.Id != ContextLogicRuleDomainLogic.DefaultExecutionContextGroup)){
                var modelNode = contextsGroup as IModelNode;
                if (modelNode != null)
                    modelNode.AddNode<IModelExecutionContexts>(ContextLogicRuleDomainLogic.DefaultExecutionContextGroup);
            }
        }
    }
}