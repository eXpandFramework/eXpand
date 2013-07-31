using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Model.VisibilityCalculators {
    public class AlwaysEditableVisibilityCalculator : IModelIsReadOnly {
        public bool IsReadOnly(IModelNode node, string propertyName) {
            return false;
        }

        public bool IsReadOnly(IModelNode node) {
            return false;
        }
    }
}