using System.ComponentModel;

namespace eXpand.ExpressApp.ModelArtifactState.StateInfos
{
    /// <summary>
    /// Arguments of the artifactStateCustomizing event.
    /// </summary>
    public class ArtifactStateInfoCustomizingEventArgs : CancelEventArgs
    {
        public ArtifactStateInfoCustomizingEventArgs(ArtifactStateInfo info, bool cancel)
        {
            ArtifactStateInfo = info;
            Cancel = cancel;
        }

        /// <summary>
        /// Allows you to customize the information about the artifact states.
        /// </summary>
        public ArtifactStateInfo ArtifactStateInfo { get; set; }
    }
}