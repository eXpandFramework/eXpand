using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Model.VisibilityCalculators {
    public class NotVisibileCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return false;
        }
    }
}