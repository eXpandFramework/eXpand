using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model {
    public class ObjectViewActionExecutionContextsGroupNodeUpdater : ModelNodesGeneratorUpdater<ActionExecutionContextsGroupNodeGenerator> {
        public const string ObjectViewActionContext = "ObjectViewActionContext";
        public override void UpdateNode(ModelNode node) {
            if (node.GetParent<IModelArtifactState>()==null){
                var modelActionExecutionContextGroup = ((IModelActionExecutionContextGroup)node);
                var modelActionExecutionContexts = modelActionExecutionContextGroup.AddNode<IModelActionExecutionContexts>(ObjectViewActionContext);
                modelActionExecutionContexts.AddNode<IModelActionExecutionContext>("New");
            }
        }
    }
}
