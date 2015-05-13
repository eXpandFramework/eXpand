using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model.VisibilityCalculators{
    public class ActionVisibilityCalculator<T> : IModelIsVisible where T : ActionBase {
        public bool IsVisible(IModelNode node, string propertyName) {
            return node.GetParent<IModelAction>().ToAction().GetType() == typeof(T);
        }
    }
}