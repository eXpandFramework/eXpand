using System;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;

namespace eXpand.ExpressApp.Win.PropertyEditors.LookupPropertyEditor
{
    public class LookupEdit : DevExpress.ExpressApp.Win.Editors.LookupEdit
    {
        static LookupEdit()
        {
            RepositoryItemLookupEdit.Register();
        }



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
            args.DialogController.Cancelling += DialogController_Cancelling;

            args.View =
                ObjectEditorHelper.CreateDetailView(Properties.Helper.Application, Properties.Helper.ObjectSpace, EditValue,
                                                    Properties.Helper.LookupObjectType, Properties.ReadOnly);
        }

        private void DialogController_Cancelling(object sender, EventArgs e)
        {
            var controller = (DialogController)sender;
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
}