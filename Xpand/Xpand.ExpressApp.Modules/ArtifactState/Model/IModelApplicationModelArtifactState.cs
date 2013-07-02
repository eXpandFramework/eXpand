using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.ArtifactState.Model {
    public interface IModelApplicationModelArtifactState : IModelNode {
        IModelArtifactState ModelArtifactState { get; }
    }
}