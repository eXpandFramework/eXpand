using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;

namespace eXpand.ExpressApp.Win.PropertyEditors.BooleanPropertyEditor{
    [PropertyEditor(typeof(bool?))]
    public class BooleanPropertyEditor : DevExpress.ExpressApp.Win.Editors.BooleanPropertyEditor
    {
        public BooleanPropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info)
        {
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            ((RepositoryItemBooleanEdit) item).AllowGrayed = true;
        }
    }
}