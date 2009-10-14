using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;

namespace eXpand.ExpressApp.Win.PropertyEditors.StringPropertyEditor
{
    public class StringDisableTextEditorPropertyEditor:DevExpress.ExpressApp.Win.Editors.StringPropertyEditor
    {
        public StringDisableTextEditorPropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info) {
        }
        protected override DevExpress.XtraEditors.Repository.RepositoryItem CreateRepositoryItem() {
            var item = base.CreateRepositoryItem();
            if (item is RepositoryItemPredefinedValuesStringEdit)
                ((RepositoryItemPredefinedValuesStringEdit) item).TextEditStyle=TextEditStyles.DisableTextEditor;
            return item;
        }
    }
}
