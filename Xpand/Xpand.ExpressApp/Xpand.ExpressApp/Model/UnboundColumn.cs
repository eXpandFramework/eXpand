using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Model {
    public interface IModelColumnUnbound : IModelColumn {
        [Category("eXpand")]
        bool ShowUnboundExpressionMenu { get; set; }
        [Category("eXpand")]
        [Required]
        string UnboundExpression { get; set; }

        [ModelBrowsable(typeof(ModelPropertyEditorTypeVisibilityCalculator))]
        new string PropertyName { get; set; }
        [Localizable(true)]
        [Description("Specifies the caption of the current Property Editor.")]
        [Category("eXpand")]
        [Required]
        new string Caption { get; set; }
    }
    public class ModelPropertyEditorTypeVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return propertyName != "PropertyEditorType" && propertyName != "PropertyName";
        }
    }
    [DomainLogic(typeof(IModelColumnUnbound))]
    public class IModelColumnUnboundLogic {
        public static string Get_PropertyName(IModelColumnUnbound columnUnbound) {
            return ((IModelListView)columnUnbound.Parent.Parent).ModelClass.KeyProperty;
        }
    }

}