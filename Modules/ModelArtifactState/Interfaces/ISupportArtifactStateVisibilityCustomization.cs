using eXpand.ExpressApp.ModelArtifactState.StateInfos;

namespace eXpand.ExpressApp.ModelArtifactState.Interfaces{
    public interface ISupportArtifactStateVisibilityCustomization : ISupportArtifactState
    {
        void CustomizeVisibility(ArtifactStateInfo info);
    }
}