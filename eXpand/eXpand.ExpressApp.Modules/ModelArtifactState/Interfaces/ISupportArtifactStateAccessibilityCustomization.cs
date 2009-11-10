using eXpand.ExpressApp.ModelArtifactState.StateInfos;

namespace eXpand.ExpressApp.ModelArtifactState.Interfaces{
    public interface ISupportArtifactStateAccessibilityCustomization:ISupportArtifactState
    {
        void CustomizeAccessibility(ArtifactStateInfo info);
    }
}