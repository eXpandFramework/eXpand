using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    [PropertyEditor(typeof(TimeSpan?),true)]
    [PropertyEditor(typeof(TimeSpan),true)]
    public class XpandTimeSpanPropertyEditor : TimeSpanPropertyEditor {
        public XpandTimeSpanPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
        }
        protected override void SetupRepositoryItem(DevExpress.XtraEditors.Repository.RepositoryItem item) {
            base.SetupRepositoryItem(item);
            var repositoryItemTimeSpanEdit = ((RepositoryItemTimeSpanEdit) item);
            repositoryItemTimeSpanEdit.AllowNullInput=MemberInfo.MemberType.IsNullableType()?DefaultBoolean.True : DefaultBoolean.Default;
            repositoryItemTimeSpanEdit.NullText = null;
        }

        protected override object CreateControlCore() {
            var controlCore = (BaseEdit) base.CreateControlCore();
            controlCore.EditValueChanged += ControlOnEditValueChanged;
            return controlCore;
        }

        protected override void OnControlValueChanged() {
            base.OnControlValueChanged();
            if (Equals(Control.EditValue,TimeSpan.Zero )&&MemberInfo.MemberType.IsNullableType()) {
                Control.EditValue = null;
            }
        }

        void ControlOnEditValueChanged(object sender, EventArgs eventArgs) {
            if (ReferenceEquals(Control.EditValue, "")&& MemberInfo.MemberType.IsNullableType()) {
                Control.EditValue = null;
            }
        }
    }
}
