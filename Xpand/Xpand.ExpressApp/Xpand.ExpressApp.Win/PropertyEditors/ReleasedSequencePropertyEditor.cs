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
using Xpand.Persistent.Base.General.CustomAttributes;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(string), EditorAliases.ReleasedSequence, false)]
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(int), EditorAliases.ReleasedSequence, false)]
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(double), EditorAliases.ReleasedSequence, false)]
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(float), EditorAliases.ReleasedSequence, false)]
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(long), EditorAliases.ReleasedSequence, false)]
    public class ReleasedSequencePropertyEditor : DXPropertyEditor, IComplexViewItem, IReleasedSequencePropertyEditor {
        public ReleasedSequencePropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }
        private ObjectEditorHelper _helper;
        private XafApplication _application;
        private IObjectSpace _objectSpace;
        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            var objectEditItem = (RepositoryItemObjectEdit)item;
            objectEditItem.Init(_application, MemberInfo.MemberTypeInfo, _helper.DisplayMember, _objectSpace, _helper);
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
            if (_helper == null){
                _helper = new ObjectEditorHelper(MemberInfo.MemberTypeInfo, Model);                
            }
            _application = application;
            _objectSpace = objectSpace;
        }
        protected override object CreateControlCore() {
            return new ReleasedSequenceEdit{PopupWindowHelper = {Model = (IModelPropertyEditor) Model}};
        }


        public new ReleasedSequenceEdit Control => (ReleasedSequenceEdit)base.Control;
    }
    [ToolboxItem(false)]
    public class ReleasedSequenceEdit : ButtonEdit, IGridInplaceEdit {


        ReleaseSequencePopupWindowHelper _popupWindowHelper=new ReleaseSequencePopupWindowHelper();

        protected override void CreateRepositoryItem() {
            base.CreateRepositoryItem();
            _popupWindowHelper.ViewShowing += PopupWindowHelperOnViewShowing;
        }

        void PopupWindowHelperOnViewShowing(object sender, EventArgs eventArgs) {
            _popupWindowHelper.Init(Properties.ObjectSpace, EditingObject, Properties.Application);
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



        protected internal static string EditorName => typeof(ReleasedSequenceEdit).Name;

        public override string EditorTypeName => EditorName;

        public override object EditValue {
            get { return base.EditValue; }
            set {
                if ((value != null) && (value != DBNull.Value) && !Properties.EditValueTypeInfo.Type.IsInstanceOfType(value)) {
                    throw new InvalidCastException(string.Format(
                        SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToCast),
                        value.GetType(), Properties.EditValueTypeInfo.Type));
                }
                base.EditValue = value;
            }
        }
        public new RepositoryItemObjectEdit Properties => (RepositoryItemObjectEdit)base.Properties;

        public object EditingObject => BindingHelper.GetEditingObject(this);

        object IGridInplaceEdit.GridEditingObject { get; set; }

        public ReleaseSequencePopupWindowHelper PopupWindowHelper{
            get{ return _popupWindowHelper; }
            set{ _popupWindowHelper = value; }
        }
    }

    public class ReleaseSequencePopupWindowHelper : ExpressApp.Editors.ReleaseSequencePopupWindowHelper {
        protected override void ShowObjectCore() {
            var helper = new PopupWindowShowActionHelper(ShowObjectAction);
            helper.ShowPopupWindow();
        }

    }
}
