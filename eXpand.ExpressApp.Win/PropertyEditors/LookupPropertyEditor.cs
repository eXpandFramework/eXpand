using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace eXpand.ExpressApp.Win.PropertyEditors
{
    public class LookupEdit : DevExpress.ExpressApp.Win.Editors.LookupEdit
    {
//        private readonly XafApplication application;
//        private readonly Type editValueType;
//        private readonly ObjectSpace objectSpace;

//        public LookupEdit(XafApplication application, ObjectSpace objectSpace, Type editValueType)
//        {
//            this.application = application;
//            this.objectSpace = objectSpace;
//            this.editValueType = editValueType;
//        }


        protected override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo)
        {
            if (buttonInfo.Button.Kind == ButtonPredefines.Ellipsis)
                showPopup();

            base.OnClickButton(buttonInfo);
        }

        private void showObjectAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs args)
        {
            args.PopupWindow.View.ObjectSpace.CommitChanges();
            EditValue = null;
            EditValue = Properties.Helper.ObjectSpace.GetObject(args.PopupWindow.View.CurrentObject);
            IsModified = true;
            UpdateMaskBoxDisplayText();
            RaiseEditValueChanged();
        }

        public new RepositoryItemLookupEdit Properties
        {
            get { return (RepositoryItemLookupEdit)base.Properties; }
        }

        public override string EditorTypeName
        {
            get { return RepositoryItemLookupEdit.EditorName; }
        }

        private void showObjectAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs args)
        {
            var objectEditorHelper = new ObjectEditorHelper();
            args.DialogController.Cancelling += DialogController_Cancelling;
            
            args.View =
                objectEditorHelper.CreateDetailView(Properties.Helper.Application, Properties.Helper.ObjectSpace, EditValue,
                                        Properties.Helper.LookupObjectType, Properties.ReadOnly);
        }

        private void DialogController_Cancelling(object sender, EventArgs e)
        {
            var controller = (DialogController) sender;
            controller.Window.GetController<WinDetailViewController>().SuppressConfirmation = true;
            controller.CanCloseWindow = true;
        }

        private void showPopup()
        {
            var action = new PopupWindowShowAction(null, "MyShowObject", "");
            try
            {
                action.Application = Properties.Helper.Application;
                action.IsModal = true;
                action.CustomizePopupWindowParams += showObjectAction_CustomizePopupWindowParams;
                action.Execute += showObjectAction_Execute;
                new PopupWindowShowActionHelper(action).ShowPopupWindow();
            }
            finally
            {
                action.Dispose();
            }
        }
    }
    public class RepositoryItemLookupEdit:DevExpress.ExpressApp.Win.Editors.RepositoryItemLookupEdit
    {
        internal const string EditorName = "eXpandLookupEdit";
        public new static void Register()
        {
            if (!EditorRegistrationInfo.Default.Editors.Contains(EditorName))
            {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(LookupEdit),
                    typeof(RepositoryItemLookupEdit), typeof(PopupBaseEditViewInfo),
                    new ButtonEditPainter(), true, EditImageIndexes.LookUpEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
            }
        }
        static RepositoryItemLookupEdit() {
			Register();
		}
        //public void Init(XafApplication application, ITypeInfo editValueTypeInfo, IMemberInfo defaultMember, ObjectSpace objectSpace)
        //{
        //    this.application = application;
        //    this.editValueTypeInfo = editValueTypeInfo;
        //    this.defaultMember = defaultMember;
        //    this.objectSpace = objectSpace;
        //}

        public override string EditorTypeName { get { return EditorName; } }    
    }
    public class LookupPropertyEditor : DevExpress.ExpressApp.Win.Editors.LookupPropertyEditor
    {
        private EditorButton editButton;
        private DevExpress.ExpressApp.Win.Editors.RepositoryItemLookupEdit repositoryItemLookupEdit;

        public LookupPropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info)
        {
        }

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemLookupEdit();
        }


        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            
            repositoryItemLookupEdit = (DevExpress.ExpressApp.Win.Editors.RepositoryItemLookupEdit) item;
            repositoryItemLookupEdit.Init(DisplayFormat, Helper);
            repositoryItemLookupEdit.EditValueChanged += RepositoryItemLookupEdit_OnEditValueChanged;
            editButton = new EditorButton(ButtonPredefines.Ellipsis, "", 20, true, true, true,
                                          HorzAlignment.Default, null,
                                          new KeyShortcut(Keys.Space),
                                          "Press this button or (Spacebar) to edit the object");
            if (View != null) View.ControlsCreated += View_OnControlsCreated;
            repositoryItemLookupEdit.Buttons.Add(editButton);
            ControlValueChanged += baseControlValueChanged;
        }

        private void RepositoryItemLookupEdit_OnEditValueChanged(object sender, EventArgs e)
        {
            try
            {
                editButton.Visible = ControlValue != null;
            }
            catch
            {
            }
        }


        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            editButton.Visible = ControlValue != null;
        }

        private void baseControlValueChanged(object sender, EventArgs e)
        {
            editButton.Visible = ControlValue != null;
        }



        protected override object CreateControlCore()
        {
            var edit = new LookupEdit();

            return edit;
        }


    }
}