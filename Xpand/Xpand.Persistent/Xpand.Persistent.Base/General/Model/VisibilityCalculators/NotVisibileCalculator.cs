using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model.VisibilityCalculators {
    public class NotVisibileCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return false;
        }
    }
}