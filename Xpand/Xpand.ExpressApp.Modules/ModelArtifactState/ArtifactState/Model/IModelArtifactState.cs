using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model {
    public interface IModelArtifactState : IModelNode {
        IModelLogicConditionalControllerState ConditionalControllerState { get; }
        IModelLogicConditionalActionState ConditionalActionState { get; }        
    }
}