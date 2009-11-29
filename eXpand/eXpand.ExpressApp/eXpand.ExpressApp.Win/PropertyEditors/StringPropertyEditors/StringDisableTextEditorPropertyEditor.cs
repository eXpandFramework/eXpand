using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;

namespace eXpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors {
    public class StringDisableTextEditorPropertyEditor:StringPropertyEditor
    {
        public StringDisableTextEditorPropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info) {
        }
        protected override void SetupRepositoryItem(DevExpress.XtraEditors.Repository.RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            if (item is RepositoryItemPredefinedValuesStringEdit)
                ((RepositoryItemPredefinedValuesStringEdit)item).TextEditStyle = TextEditStyles.DisableTextEditor;
            
        }
    }
}