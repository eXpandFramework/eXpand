using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.ExpressApp.Model.VisibilityCalculators {
    public class RuntimeOnlyCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return !DesignerOnlyCalculator.IsRunFromDesigner;
        }
    }
}