using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model.RequiredCalculators {
    public class NotRequiredCalculator:IModelIsRequired {
        public bool IsRequired(IModelNode node, string propertyName) {
            return false;
        }
    }
}
