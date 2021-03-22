using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;
using Xpand.Extensions.ReflectionExtensions;


namespace Xpand.ExpressApp.Win.PropertyEditors.NullAble.BooleanPropertyEditor {
    [PropertyEditor(typeof(bool?),Persistent.Base.General.EditorAliases.NullAbleBooleanPropertyEditor, true)]
    [PropertyEditor(typeof(bool), Persistent.Base.General.EditorAliases.NullAbleBooleanPropertyEditor, true)]
    public class XpandBooleanPropertyEditor : DevExpress.ExpressApp.Win.Editors.BooleanPropertyEditor {
        public XpandBooleanPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            ControlBindingProperty = "EditValue";
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            if (item is RepositoryItemBooleanEdit edit)
                edit.AllowGrayed = MemberInfo.MemberType.IsNullableType();
        }
    }
}