using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using PopupWindow = DevExpress.ExpressApp.Web.PopupWindow;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    public class ReleaseSequencePopupWindowHelper : ExpressApp.Editors.ReleaseSequencePopupWindowHelper {
        protected override void ShowObjectCore() {

        }
    }


    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(string), false)]
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(int), false)]
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(double), false)]
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(float), false)]
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(long), false)]
    public class ReleasedSequencePropertyEditor : ASPxObjectPropertyEditorBase, IRegisterClientScript, IReleasedSequencePropertyEditor {

        private ObjectEditorHelper _helper;
        private PopupWindowShowAction _objectWindowAction;
        private ASPxButtonEdit _control;
        private bool _buttonEditTextEnabled = true;
        readonly ReleaseSequencePopupWindowHelper _popupWindowHelper = new ReleaseSequencePopupWindowHelper();
        private void DialogController_Cancelling(object sender, EventArgs e) {
            var controller = ((DialogController)sender);
            controller.CanCloseWindow = true;
            objectSpace.Committing -= ObjectSpaceOnCommitting;
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs e) {
            _popupWindowHelper.Capture((XPObjectSpace)sender);
        }

        private void objectWindowAction_OnCustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
            args.DialogController.Cancelling += DialogController_Cancelling;
            objectSpace.Committing += ObjectSpaceOnCommitting;
            args.View = _popupWindowHelper.CreateListView(application, objectSpace, (ISupportSequenceObject)View.CurrentObject);
        }
        private void objectWindowAction_OnExecute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
            _popupWindowHelper.Assign(args, (ISupportSequenceObject)View.CurrentObject);
            string displayValue = GetPropertyDisplayValue();
            ((PopupWindow)args.PopupWindow).ClosureScript = "if(window.opener != null) window.opener.resultObject = '" + (displayValue != null ? displayValue.Replace("'", "\\'") : "") + "';";
        }
        private void SetButtonEditTextEnabled(bool value) {
            if (_buttonEditTextEnabled != value) {
                _buttonEditTextEnabled = value;
                if (value) {
                    _control.CssClass=_control.CssClass.Replace(" dxDisabled", "");
                } else {
                    _control.CssClass += " dxDisabled";
                }
            }
        }
        private string GetClientScript(ASPxButtonEdit editor) {
            string resultFunc = @"
			    window.ProcessObjectEditResult" + editor.ClientID + @" = function() {
				    window." + editor.ClientID + @".SetText(window.resultObject);
			    }";
            return resultFunc;
        }
        private void buttonEdit_Load(object sender, EventArgs e) {
            var editor = (ASPxButtonEdit)sender;
            ((Control)sender).Load -= buttonEdit_Load;
            string resultFunc = GetClientScript(editor);
            editor.Page.ClientScript.RegisterClientScriptBlock(GetType(), "ButtonEditScript" + editor.ClientID, resultFunc, true);
            string handler = string.Format("function Add_{0}_ButtonClickHandler(sender, e)", MemberInfo.Name.Replace(".", ""));
            handler += "{" + application.PopupWindowManager.GenerateModalOpeningScript(editor, _objectWindowAction, ASPxLookupPropertyEditor.WindowWidth, ASPxLookupPropertyEditor.WindowHeight, false, GetButtonEditProcessResultFunction()) + "; }";
            editor.ClientSideEvents.ButtonClick = handler;
        }
        private string GetButtonEditProcessResultFunction() {
            return "ProcessObjectEditResult" + _control.ClientID + "()";
        }
        protected override object GetControlValueCore() {
            return MemberInfo.GetValue(CurrentObject);
        }
        protected override bool IsMemberSetterRequired() {
            return false;
        }
        protected override WebControl CreateEditModeControlCore() {
            if (_objectWindowAction == null) {
                _objectWindowAction = new PopupWindowShowAction(null, "ShowObjectDetailViewPopup", PredefinedCategory.Unspecified.ToString());
                _objectWindowAction.Execute += objectWindowAction_OnExecute;
                _objectWindowAction.CustomizePopupWindowParams += objectWindowAction_OnCustomizePopupWindowParams;
                _objectWindowAction.Application = application;
            }
            _control = new ASPxButtonEdit { EnableClientSideAPI = true };
            _control.Load += buttonEdit_Load;
            _control.Buttons.Clear();
            _control.Buttons.Add("");
            ASPxImageHelper.SetImageProperties(_control.Buttons[0].Image, "Editor_Edit");
            if (_control.Buttons[0].Image.IsEmpty) {
                _control.Buttons[0].Text = "Edit";
            }
            _control.Enabled = true;
            _control.ReadOnly = true;
            return _control;
        }
        protected override void Dispose(bool disposing) {
            try {
                if (disposing) {
                    if (_objectWindowAction != null) {
                        _objectWindowAction.Execute -= objectWindowAction_OnExecute;
                        _objectWindowAction.CustomizePopupWindowParams -= objectWindowAction_OnCustomizePopupWindowParams;
                        _objectWindowAction.Dispose();
                        _objectWindowAction = null;
                    }
                    if (_control != null) {
                        _control.Buttons.Clear();
                        _control.Dispose();
                        _control = null;
                    }
                }
            } finally {
                base.Dispose(disposing);
            }
        }
        protected override IJScriptTestControl GetEditorTestControlImpl() {
            return new JSASPxButtonEditTestControl();
        }
        protected override void ApplyReadOnly() {
            if (Editor != null && Editor.Controls.Count > 0) {
                SetButtonEditTextEnabled(AllowEdit);
                _control.Buttons[0].Enabled = PropertyValue != null || AllowEdit;
            }
        }
        protected override void ReadEditModeValueCore() {
            _control.Value = GetPropertyDisplayValue();
        }
        protected override string GetPropertyDisplayValue() {
            return _helper.GetDisplayText(MemberInfo.GetValue(CurrentObject), EmptyValue, DisplayFormat);
        }
        public ReleasedSequencePropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            skipEditModeDataBind = true;
        }
        public override void Setup(IObjectSpace space, XafApplication app) {
            base.Setup(space, app);
            if (_helper == null) {
                _helper = new ObjectEditorHelper(MemberInfo.MemberTypeInfo, Model);
            }
        }
        public override void BreakLinksToControl(bool unwireEventsOnly) {
            if (!unwireEventsOnly) {
                _control = null;
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }

        #region IRegisterClientScript Members
        public string GetClientScript() {
            string result = "";
            if (_control != null && ViewEditMode == ViewEditMode.Edit) {
                result = GetClientScript(_control);
            }
            return result;
        }
        #endregion
        #region base


        #endregion

    }
}
