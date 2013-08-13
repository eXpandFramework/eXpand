using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model {
    public interface IModelApplicationConditionalObjectView : IModelNode {
        IModelLogicConditionalObjectView ConditionalObjectView { get; }
    }
}
