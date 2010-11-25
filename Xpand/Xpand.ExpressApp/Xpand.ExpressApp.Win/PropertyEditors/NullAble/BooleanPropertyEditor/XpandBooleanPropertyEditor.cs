using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;

namespace Xpand.ExpressApp.Win.PropertyEditors.NullAble.BooleanPropertyEditor{
    [PropertyEditor(typeof(bool?))]
    [PropertyEditor(typeof(bool))]
    public class XpandBooleanPropertyEditor : DevExpress.ExpressApp.Win.Editors.BooleanPropertyEditor
    {
        public XpandBooleanPropertyEditor(Type objectType, IModelMemberViewItem model)
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