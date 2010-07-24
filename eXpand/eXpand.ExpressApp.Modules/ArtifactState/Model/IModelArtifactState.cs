using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.ArtifactState.Model {
    public interface IModelArtifactState : IModelNode
    {
        IModelLogicConditionalControllerState ConditionalControllerState { get; }
        IModelLogicConditionalActionState ConditionalActionState { get; }
        [Browsable(false)]
        IModelLogic DummyLogic { get; }
    }
}