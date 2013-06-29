using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ArtifactState.Model {
    public interface IModelArtifactState : IModelNode {
        IModelLogicConditionalControllerState ConditionalControllerState { get; }
        IModelLogicConditionalActionState ConditionalActionState { get; }
        [Browsable(false)]
        IModelLogic DummyLogic { get; }
    }
}