using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.ArtifactState.Model {
    public interface IModelApplicationModelArtifactState : IModelNode
    {
        IModelArtifactState ModelArtifactState { get; set; }
    }
}