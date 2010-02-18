using eXpand.ExpressApp.ModelArtifactState.ArtifactState;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState {
    public interface IActionStateRule : IArtifactRule {
        string ActionId { get; set; }
        ActionState ActionState { get; set; }
    }
}