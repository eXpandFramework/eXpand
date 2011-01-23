using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using Xpand.ExpressApp.Editors;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.PropertyEditors {

    [PropertyEditor(typeof(string), false)]
    [PropertyEditor(typeof(int), false)]
    [PropertyEditor(typeof(double), false)]
    [PropertyEditor(typeof(float), false)]
    [PropertyEditor(typeof(long), false)]
    public class ReleasedSequencePropertyEditor : DXPropertyEditor, IComplexPropertyEditor, IReleasedSequencePropertyEditor {
        public ReleasedSequencePropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }
        private ObjectEditorHelper helper;
        private XafApplication _application;
        private IObjectSpace _objectSpace;
        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            var objectEditItem = (RepositoryItemObjectEdit)item;
            objectEditItem.Init(_application, MemberInfo.MemberTypeInfo, helper.DisplayMember, _objectSpace, helper);
        }
        protected override RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemObjectEdit();
        }
        protected override void ReadValueCore() {
            base.ReadValueCore();
            Control.UpdateText();
        }
        protected override bool IsMemberSetterRequired() {
            return false;
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            if (helper == null) {
                helper = new ObjectEditorHelper(MemberInfo.MemberTypeInfo, Model);
            }
            _application = application;
            _objectSpace = objectSpace;
        }
        protected override object CreateControlCore() {
            var releasedSequenceEdit = new ReleasedSequenceEdit();
            return releasedSequenceEdit;
        }


        public new ReleasedSequenceEdit Control {
            get { return (ReleasedSequenceEdit)base.Control; }
        }
    }
    [ToolboxItem(false)]
    public class ReleasedSequenceEdit : ButtonEdit, IGridInplaceEdit {


        ReleaseSequencePopupWindowHelper _popupWindowHelper;
        protected override void CreateRepositoryItem() {
            base.CreateRepositoryItem();
            _popupWindowHelper = new ReleaseSequencePopupWindowHelper();
            _popupWindowHelper.ViewShowing += PopupWindowHelperOnViewShowing;
        }

        void PopupWindowHelperOnViewShowing(object sender, EventArgs eventArgs) {
            _popupWindowHelper.Init(Properties.ObjectSpace, (ISupportSequenceObject)EditingObject, Properties.Application);
        }

        protected internal void UpdateText() {
            UpdateDisplayText();
            Invalidate();
        }
        protected override void OnEditorKeyDown(KeyEventArgs e) {
            base.OnEditorKeyDown(e);
            if (e.KeyData == Keys.Space) {
                _popupWindowHelper.ShowObject();
            }
        }
        protected override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo) {
            base.OnClickButton(buttonInfo);
            if (buttonInfo.Button.IsDefaultButton) {
                _popupWindowHelper.ShowObject();
            }
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e) {
            base.OnMouseDoubleClick(e);
            EditHitInfo info = ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
            if (info.HitTest == EditHitTest.None || info.HitTest == EditHitTest.MaskBox) {
                _popupWindowHelper.ShowObject();
            }
        }
        static ReleasedSequenceEdit() {
            if (!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(ReleasedSequenceEdit),
                    typeof(RepositoryItemObjectEdit), typeof(ButtonEditViewInfo),
                    new ButtonEditPainter(), true, EditImageIndexes.ButtonEdit, typeof(DevExpress.Accessibility.ButtonEditAccessible)));
            }
        }



        protected internal static string EditorName {
            get { return typeof(ReleasedSequenceEdit).Name; }
        }

        public override string EditorTypeName {
            get { return EditorName; }
        }
        public override object EditValue {
            get { return base.EditValue; }
            set {
                if ((value != null) && (value != DBNull.Value) && !Properties.EditValueTypeInfo.Type.IsAssignableFrom(value.GetType())) {
                    throw new InvalidCastException(string.Format(
                        SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToCast),
                        value.GetType(), Properties.EditValueTypeInfo.Type));
                }
                base.EditValue = value;
            }
        }
        public new RepositoryItemObjectEdit Properties {
            get { return (RepositoryItemObjectEdit)base.Properties; }
        }
        public object EditingObject {
            get { return BindingHelper.GetEditingObject(this); }
        }

        object IGridInplaceEdit.GridEditingObject { get; set; }
    }

    public class ReleaseSequencePopupWindowHelper:ExpressApp.Editors.ReleaseSequencePopupWindowHelper {
        protected override void ShowObjectCore() {
            var helper = new PopupWindowShowActionHelper(ShowObjectAction);
            helper.ShowPopupWindow();
        }
    }
}
