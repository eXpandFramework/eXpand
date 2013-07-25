using System;
using System.ComponentModel;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Model {
    [ModelAbstractClass]
    public interface IModelMemberEx : IModelMember {
        [Category("eXpand")]
        [Description("Specifies the current property type.")]
        [TypeConverter(typeof(XpandStringToTypeConverterExtended))]
        [ModelBrowsable(typeof(ModelTypeVisibilityCalculator))]
        [Required]
        [ModelReadOnly(typeof(AlwaysEditableVisibilityCalculator))]
        new Type Type { get; set; }
        [ModelBrowsable(typeof(DesignerOnlyCalculatorForNativeMembers))]
        new bool IsCustom { get; set; }
        [Browsable(false)]
        object Tag { get; set; }
    }

    public class XpandStringToTypeConverterExtended : DevExpress.ExpressApp.Model.Core.StringToTypeConverterExtended {

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value != null) {
                ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(Type.GetType(value.ToString()));
                return typeInfo != null ? typeInfo.Type : base.ConvertFrom(context,culture, value);
            }
            return null;
        }
    }

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

    public class DesignerOnlyCalculatorForNativeMembers : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return !(node is IModelRuntimeMember);
        }
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
    public class AlwaysEditableVisibilityCalculator : IModelIsReadOnly {
        public bool IsReadOnly(IModelNode node, string propertyName) {
            return false;
        }

        public bool IsReadOnly(IModelNode node) {
            return false;
        }
    }

    public class ModelTypeVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return !(node is IModelRuntimeOrphanedColection) || propertyName != "Type";
        }
    }

}
