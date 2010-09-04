using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.ArtifactState.Model {
    public interface IModelArtifactState : IModelNode
    {
        IModelLogicConditionalControllerState ConditionalControllerState { get; }
        IModelLogicConditionalActionState ConditionalActionState { get; }
        [Browsable(false)]
        IModelLogic DummyLogic { get; }
    }
}