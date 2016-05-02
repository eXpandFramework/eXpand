using System;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Web;

namespace Xpand.ExpressApp.Web.PropertyEditors{
    [PropertyEditor(typeof(object), false)]
    public class ASPxLookupPropertyEditorWithEdit : ASPxLookupPropertyEditor {
        public ASPxLookupPropertyEditorWithEdit(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        private PopupWindowShowAction _editObjectAction;

        protected override WebControl CreateEditModeControlCore() {
            WebControl control = base.CreateEditModeControlCore();
            ASPxButtonEditBase buttonEdit;
            if (UseFindEdit()) {
                buttonEdit = FindEdit.Editor;
            }
            else {
                buttonEdit = DropDownEdit.DropDown;
            }
            if (_editObjectAction == null) {
                _editObjectAction = new PopupWindowShowAction(null, MemberInfo.Name + "_ASPxLookupEditor_EditObject", PredefinedCategory.Unspecified);
                _editObjectAction.CustomizePopupWindowParams += editObjectAction_CustomizePopupWindowParams;
                _editObjectAction.Application = application;
            }
            EditButton editButton = new EditButton();
            ASPxImageHelper.SetImageProperties(editButton.Image, "Action_Edit_12x12");
            buttonEdit.Buttons.Add(editButton);
            buttonEdit.Load += buttonEdit_Load;
            return control;
        }

        void buttonEdit_Load(object sender, EventArgs e) {
            ASPxButtonEditBase buttonEdit = (ASPxButtonEditBase)sender;
            string showModalWindowScript = application.PopupWindowManager.GetShowPopupWindowScript(_editObjectAction,
                null, buttonEdit.ClientID, false, true, UseFindEdit(),false);
            ButtonEditClientSideEventsBase clientSideEvents = (ButtonEditClientSideEventsBase)buttonEdit.GetClientSideEvents();
            int index = clientSideEvents.ButtonClick.LastIndexOf("}", StringComparison.Ordinal);
            string script = String.Format("if(e.buttonIndex == 2 && s.GetText() != '{0}') {{ {1} e.handled = true; e.processOnServer = false; }}", CaptionHelper.NullValueText, showModalWindowScript);
            clientSideEvents.ButtonClick = clientSideEvents.ButtonClick.Insert(index, script);
        }

        void editObjectAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            IObjectSpace editObjectSpace = objectSpace.CreateNestedObjectSpace();
            DetailView view = application.CreateDetailView(editObjectSpace, editObjectSpace.GetObject(GetControlValueCore()));
            view.ViewEditMode = ViewEditMode.Edit;
            e.View = view;
            e.DialogController.SaveOnAccept = true;
        }
    }
}