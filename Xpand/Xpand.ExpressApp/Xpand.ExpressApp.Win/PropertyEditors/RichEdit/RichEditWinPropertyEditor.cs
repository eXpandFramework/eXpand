using System;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;

namespace Xpand.ExpressApp.Win.PropertyEditors.RichEdit {
    [PropertyEditor(typeof(string), false)]
    public class RichEditWinPropertyEditor : WinPropertyEditor, IInplaceEditSupport {
        RichEditContainer _richEditContainer;

        public RichEditWinPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        protected override object CreateControlCore() {
            _richEditContainer = new RichEditContainer();
            ControlBindingProperty = "RtfText";
            return _richEditContainer;
        }

        protected override void OnAllowEditChanged() {
            base.OnAllowEditChanged();
            if (_richEditContainer != null)
                _richEditContainer.RichEditControl.ReadOnly = !AllowEdit;
        }

        #region IInplaceEditSupport Members

        public DevExpress.XtraEditors.Repository.RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemRtfEditEx();
        }

        #endregion


    }

}
