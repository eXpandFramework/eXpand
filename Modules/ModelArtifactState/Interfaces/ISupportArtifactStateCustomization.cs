using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.StateInfos;

namespace eXpand.ExpressApp.ModelArtifactState.Interfaces{
    public interface ISupportArtifactStateCustomization
    {
        bool IsReady { get; }
        void ForceCustomization(bool isReady,View view);
        void CustomizeArtifactState(ArtifactStateInfo info);
        event EventHandler<ArtifactStateInfoCustomizingEventArgs> ArtifactStateCustomizing;
        event EventHandler<ArtifactStateInfoCustomizedEventArgs> ArtifactStateCustomized;
    }
}