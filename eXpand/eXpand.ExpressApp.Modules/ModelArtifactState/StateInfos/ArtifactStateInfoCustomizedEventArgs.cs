using System;

namespace eXpand.ExpressApp.ModelArtifactState.StateInfos
{
    /// <summary>
    /// Arguments of the ArtifactStateCustomized event.
    /// </summary>
    public class ArtifactStateInfoCustomizedEventArgs : EventArgs
    {
        

        public ArtifactStateInfoCustomizedEventArgs(ArtifactStateInfo info){
            ArtifactStateInfo = info;
        }


        /// <summary>
        /// Allows you to know the information about the artifact states.
        /// </summary>
        public ArtifactStateInfo ArtifactStateInfo { get; set; }
    }
}