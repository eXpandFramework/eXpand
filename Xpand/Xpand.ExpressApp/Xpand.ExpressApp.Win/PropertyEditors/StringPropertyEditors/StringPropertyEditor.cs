using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;
using Xpand.ExpressApp.PropertyEditors;

namespace Xpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors {
    [PropertyEditor(typeof(string), false)]
    public class StringPropertyEditor : DevExpress.ExpressApp.Win.Editors.StringPropertyEditor, IStringPropertyEditor {
        public StringPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        protected override void SetupRepositoryItem(DevExpress.XtraEditors.Repository.RepositoryItem item) {
            base.SetupRepositoryItem(item);
            var itemPredefinedValuesStringEdit = item as RepositoryItemPredefinedValuesStringEdit;
            if (itemPredefinedValuesStringEdit != null)
                itemPredefinedValuesStringEdit.TextEditStyle = TextEditStyles.DisableTextEditor;
        }
    }
}