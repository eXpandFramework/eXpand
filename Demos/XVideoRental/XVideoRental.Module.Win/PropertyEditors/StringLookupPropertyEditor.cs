using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace XVideoRental.Module.Win.PropertyEditors {
    [PropertyEditor(typeof(string), false)]
    public class StringLookupPropertyEditor : Common.Win.PropertyEditors.StringLookupPropertyEditor {
        public StringLookupPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }
    }
}
