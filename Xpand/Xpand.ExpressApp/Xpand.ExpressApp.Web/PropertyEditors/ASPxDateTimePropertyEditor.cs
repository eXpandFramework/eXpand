using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    public interface IModelMemberViewItemRelativeDate : IModelMemberViewItem {
        [Category("eXpand")]
        [ModelBrowsable(typeof(ModelMemberViewItemRelativeDateVisibilityCalculator))]
        bool DisplayRelativeDate { get; set; }
    }

    public class ModelMemberViewItemRelativeDateVisibilityCalculator:IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return typeof (ASPxDateTimePropertyEditor).IsAssignableFrom(((IModelMemberViewItem) node).PropertyEditorType);
        }
    }

    [PropertyEditor(typeof(DateTime),  false)]
    public class ASPxDateTimePropertyEditor:DevExpress.ExpressApp.Web.Editors.ASPx.ASPxDateTimePropertyEditor {
        public ASPxDateTimePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
        }
        public new IModelMemberViewItemRelativeDate Model {
            get { return (IModelMemberViewItemRelativeDate) base.Model; }
        }

        protected override string GetPropertyDisplayValue() {
            var displayValue = base.GetPropertyDisplayValue();
            return Model.DisplayRelativeDate ? DateTime.Parse(displayValue).RelativeDate() : displayValue;
        }
    }
}
