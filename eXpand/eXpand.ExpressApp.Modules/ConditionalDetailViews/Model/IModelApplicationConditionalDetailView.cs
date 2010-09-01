using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.ConditionalDetailViews.Model
{
    public interface IModelApplicationConditionalDetailView:IModelNode
    {
        IModelLogicConditionalDetailView ConditionalDetailView { get; }
    }
}
