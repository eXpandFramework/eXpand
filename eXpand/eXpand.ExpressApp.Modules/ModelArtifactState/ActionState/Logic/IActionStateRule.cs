using eXpand.ExpressApp.ModelArtifactState.ArtifactState;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public interface IActionStateRule : IArtifactRule {
        string ActionId { get; set; }
        ActionState ActionState { get; set; }
    }
}