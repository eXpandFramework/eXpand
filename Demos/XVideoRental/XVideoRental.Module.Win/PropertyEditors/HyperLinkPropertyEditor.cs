using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace XVideoRental.Module.Win.PropertyEditors {
    [PropertyEditor(typeof(String), "HyperLinkPropertyEditor", false)]
    public class HyperLinkPropertyEditor : Common.Win.PropertyEditors.HyperLinkPropertyEditor {
        public HyperLinkPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) {
        }
    }
}