using System;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraEditors.Drawing;

namespace eXpand.ExpressApp.Win.Editors
{
    public class ObjectEdit : DevExpress.ExpressApp.Win.Editors.ObjectEdit
    {
        protected override void OnEditorKeyDown(KeyEventArgs e)
        {
//            
            if (e.KeyData == Keys.F4 || e.KeyData == Keys.Return || e.KeyData == Keys.Space)
            {
                showPopup();
                e.Handled = true;
            }
            else
                base.OnEditorKeyDown(e);
        }

        protected override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo)
        {
//            base.OnClickButton(buttonInfo);
            if (buttonInfo.Button.IsDefaultButton)
                showPopup();
        }

        private void showPopup()
        {
            var action = new PopupWindowShowAction(null, "MyShowObject", "");
            try
            {
                action.Application = Properties.Application;
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

        private void showObjectAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs args)
        {
            args.PopupWindow.View.ObjectSpace.CommitChanges();
            EditValue = null;
            EditValue = Properties.ObjectSpace.GetObject(args.PopupWindow.View.CurrentObject);
            IsModified = true;
            UpdateMaskBoxDisplayText();
            RaiseEditValueChanged();
        }


        private void showObjectAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs args)
        {
            args.DialogController.Cancelling += DialogController_Cancelling;
            args.View =
                ObjectEditorHelper.CreateDetailView(Properties.Application, Properties.ObjectSpace, EditValue,
                                        Properties.EditValueTypeInfo.Type, Properties.ReadOnly);
        }

        private void DialogController_Cancelling(object sender, EventArgs e)
        {
            var controller = (DialogController) sender;
            controller.Window.GetController<WinDetailViewController>().SuppressConfirmation = true;
            controller.CanCloseWindow = true;
        }
    }
}