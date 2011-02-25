using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.PropertyEditors.NullAble.DatePropertyEditor {
    [PropertyEditor(typeof(DateTime), true)]
    [PropertyEditor(typeof(DateTime?), true)]
    public class XpandDatePropertyEditor : DevExpress.ExpressApp.Win.Editors.DatePropertyEditor {
        public XpandDatePropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            var repositoryItemIntegerEdit = (RepositoryItemDateEdit)item;
            HandleNullType(repositoryItemIntegerEdit);
            HandleFullDateTimeDisplay(repositoryItemIntegerEdit);
        }

        void HandleFullDateTimeDisplay(RepositoryItemDateEdit repositoryItemIntegerEdit) {
            var displayDateAndTime = MemberInfo.FindAttribute<DisplayDateAndTime>();
            if (displayDateAndTime!=null) {
                repositoryItemIntegerEdit.VistaDisplayMode = DefaultBoolean.True;
                repositoryItemIntegerEdit.VistaEditTime = DefaultBoolean.True;
            }
        }

        void HandleNullType(RepositoryItemDateEdit repositoryItemIntegerEdit) {
            if (View != null) {
                Type type = MemberInfo.MemberType;
                bool b = type == typeof(DateTime?);
                if (b)
                    repositoryItemIntegerEdit.NullDate = null;
                repositoryItemIntegerEdit.AllowNullInput = b ? DefaultBoolean.True : DefaultBoolean.Default;
            }
        }
    }
}