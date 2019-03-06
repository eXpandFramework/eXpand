using System;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Win.PropertyEditors{
    [Obsolete("Use Xpand.ExpressApp.Win.PropertyEditors.EnumPropertyEditor instead",true)]
    public class FilterableEnumPropertyEditor : EnumPropertyEditor {
        public FilterableEnumPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model){
            throw new NotImplementedException($"Use {typeof(EnumPropertyEditor).FullName} instead");
        }

    }
}