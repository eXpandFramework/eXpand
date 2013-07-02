using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.ConditionalObjectView.Model {
    public interface IModelApplicationConditionalObjectView : IModelNode {
        IModelLogicConditionalObjectView ConditionalObjectView { get; }
    }
}
