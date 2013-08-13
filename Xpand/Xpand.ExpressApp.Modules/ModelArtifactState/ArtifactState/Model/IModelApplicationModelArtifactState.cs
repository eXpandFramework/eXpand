using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model {
    public interface IModelApplicationModelArtifactState : IModelNode {
        IModelArtifactState ModelArtifactState { get; }
    }
}