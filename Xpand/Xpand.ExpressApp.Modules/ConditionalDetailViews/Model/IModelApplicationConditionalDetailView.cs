using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.ConditionalDetailViews.Model
{
    public interface IModelApplicationConditionalDetailView:IModelNode
    {
        IModelLogicConditionalDetailView ConditionalDetailView { get; }
    }
}
