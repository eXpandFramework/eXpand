using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model.VisibilityCalculators {
    public class WebOnlyVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return XpandModuleBase.GetIsHosted(node.Application);
        }
    }
}