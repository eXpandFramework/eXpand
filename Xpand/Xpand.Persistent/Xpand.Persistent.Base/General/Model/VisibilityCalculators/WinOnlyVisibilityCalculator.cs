using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model.VisibilityCalculators {
    public class WinOnlyVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return !node.Application.IsHosted();
        }
    }
}