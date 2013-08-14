using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model {
    public class ObjectViewActionExecutionContextsGroupNodeUpdater : ModelNodesGeneratorUpdater<ActionExecutionContextsGroupNodeGenerator> {
        public const string ObjectViewActionContext = "ObjectViewActionContext";
        public override void UpdateNode(ModelNode node) {
            var modelActionExecutionContextGroup = ((IModelActionExecutionContextGroup)node);
            var modelActionExecutionContexts = modelActionExecutionContextGroup.AddNode<IModelActionExecutionContexts>(ObjectViewActionContext);
            modelActionExecutionContexts.AddNode<IModelActionExecutionContext>("New");
        }
    }
}
