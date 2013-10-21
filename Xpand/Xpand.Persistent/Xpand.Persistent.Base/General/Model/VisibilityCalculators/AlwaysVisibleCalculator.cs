using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model.VisibilityCalculators {
    public class AlwaysVisibleCalculator:IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return true;
        }
    }
}
