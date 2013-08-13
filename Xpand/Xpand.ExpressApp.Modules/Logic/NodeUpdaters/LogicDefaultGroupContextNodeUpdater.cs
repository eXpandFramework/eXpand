using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.NodeUpdaters {
    public class LogicDefaultGroupContextNodeUpdater : ModelNodesGeneratorUpdater<ExecutionContextsGroupNodeGenerator>   {
        readonly Func<IModelApplication, IModelLogic> _modelLogic;
        

        public LogicDefaultGroupContextNodeUpdater(Func<IModelApplication, IModelLogic> modelLogic) {
            _modelLogic = modelLogic;
        }

        public override void UpdateNode(ModelNode node) {
            IModelExecutionContextsGroup m = _modelLogic.Invoke(node.Application).ExecutionContextsGroup;
            if (m[LogicRuleDomainLogic.DefaultExecutionContextGroup] != null) return;
            m.AddNode<IModelExecutionContexts>(LogicRuleDomainLogic.DefaultExecutionContextGroup);
        }
    }
}