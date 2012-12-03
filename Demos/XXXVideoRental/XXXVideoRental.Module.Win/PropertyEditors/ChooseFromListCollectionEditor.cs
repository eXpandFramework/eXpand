using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace XVideoRental.Module.Win.PropertyEditors {
    [PropertyEditor(typeof(IList<>), false)]
    public class ChooseFromListCollectionEditor : Common.Win.PropertyEditors.ChooseFromListCollectionEditor {
        public ChooseFromListCollectionEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) {
        }
    }
    [PropertyEditor(typeof(string), false)]
    public class MemoPropertyEditor : Common.Win.PropertyEditors.MemoPropertyEditor {
        public MemoPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }
    }
}
