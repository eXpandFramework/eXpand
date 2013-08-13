using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.ArtifactState.Model {
    public interface IModelArtifactState : IModelNode
    {
        IModelLogic ConditionalControllerState { get; }
        IModelLogic ConditionalActionState { get; }
    }
}