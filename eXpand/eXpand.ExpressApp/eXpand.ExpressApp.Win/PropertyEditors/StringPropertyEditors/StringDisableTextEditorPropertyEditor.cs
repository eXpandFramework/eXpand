using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;

namespace eXpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors {
    public class StringDisableTextEditorPropertyEditor:StringPropertyEditor
    {
        public StringDisableTextEditorPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
        }

        protected override void SetupRepositoryItem(DevExpress.XtraEditors.Repository.RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            if (item is RepositoryItemPredefinedValuesStringEdit)
                ((RepositoryItemPredefinedValuesStringEdit)item).TextEditStyle = TextEditStyles.DisableTextEditor;    
        }
    }
}