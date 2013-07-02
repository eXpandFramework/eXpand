using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.NodeUpdaters {
    public class LogicDefaultGroupContextNodeUpdater<TModelLogic, TModelApplication> : ModelNodesGeneratorUpdater<ExecutionContextsGroupNodeGenerator> where TModelLogic : IModelLogic where TModelApplication:IModelNode {
        readonly Func<TModelApplication, TModelLogic> _modelLogic;
        public const string Default = "Default";

        public LogicDefaultGroupContextNodeUpdater(Func<TModelApplication, TModelLogic> modelLogic) {
            _modelLogic = modelLogic;
        }

        public override void UpdateNode(ModelNode node) {
            IModelExecutionContextsGroup m = _modelLogic.Invoke((TModelApplication) node.Application).ExecutionContextsGroup;
            if (m[Default] != null) return;
            m.AddNode<IModelExecutionContexts>(Default);
        }

        
    }
}