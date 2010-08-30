using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;

namespace eXpand.ExpressApp.Win.PropertyEditors.NullAble.BooleanPropertyEditor{
    [PropertyEditor(typeof(bool?))]
    public class BooleanPropertyEditor : DevExpress.ExpressApp.Win.Editors.BooleanPropertyEditor
    {
        public BooleanPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            ((RepositoryItemBooleanEdit) item).AllowGrayed = true;
        }
    }
}