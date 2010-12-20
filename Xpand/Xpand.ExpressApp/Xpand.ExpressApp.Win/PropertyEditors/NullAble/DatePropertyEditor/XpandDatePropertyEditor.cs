using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;

namespace Xpand.ExpressApp.Win.PropertyEditors.NullAble.DatePropertyEditor {
    [PropertyEditor(typeof(DateTime))]
    [PropertyEditor(typeof(DateTime?))]
    public class XpandDatePropertyEditor : DevExpress.ExpressApp.Win.Editors.DatePropertyEditor {
        public XpandDatePropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            var repositoryItemIntegerEdit = (RepositoryItemDateEdit)item;
            if (View != null) {
                Type type = MemberInfo.MemberType;
                bool b = type == typeof(DateTime?);
                if (b)
                    repositoryItemIntegerEdit.NullDate = null;
                repositoryItemIntegerEdit.AllowNullInput =
                    b
                        ? DefaultBoolean.True
                        : DefaultBoolean.Default;
            }
        }
    }
}