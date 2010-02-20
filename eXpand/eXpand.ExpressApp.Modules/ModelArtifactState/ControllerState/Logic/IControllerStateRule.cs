using eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.ControllerState.Logic {
    public interface IControllerStateRule : IArtifactRule {
        /// <summary>
        /// Type of controller to activate or not
        /// </summary>
        string ControllerType { get; set; }
        State State { get; set; }
    }
}