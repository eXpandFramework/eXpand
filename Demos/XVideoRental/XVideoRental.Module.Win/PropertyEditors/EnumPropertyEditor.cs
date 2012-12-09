using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace XVideoRental.Module.Win.PropertyEditors {
    [PropertyEditor(typeof(Enum), true)]
    public class EnumPropertyEditor : Common.Win.PropertyEditors.EnumPropertyEditor {
        public EnumPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }
    }
}