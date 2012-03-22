using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Actions;

namespace Xpand.ExpressApp.MessageBox {
    public class GenericMessageBox {

        public delegate void MessageBoxEventHandler(object sender, ShowViewParameters e);

        private event MessageBoxEventHandler localAccept;
        private event EventHandler localCancel;

        public GenericMessageBox(ShowViewParameters svp, XafApplication app, string Message, MessageBoxEventHandler Accept, EventHandler Cancel) {
            CreateDetailView(svp, app, Message);
            AttachDialogController(svp, app, Accept, Cancel);
        }

        public GenericMessageBox(ShowViewParameters svp, XafApplication app, string Message, MessageBoxEventHandler Accept) {
            CreateDetailView(svp, app, Message);
            AttachDialogController(svp, app, Accept);
        }

        public GenericMessageBox(ShowViewParameters svp, XafApplication app, string Message) {
            CreateDetailView(svp, app, Message);
            AttachDialogController(svp, app);
        }

        private void AttachDialogController(ShowViewParameters svp, XafApplication app, MessageBoxEventHandler Accept, EventHandler Cancel) {
            localAccept = Accept;
            localCancel = Cancel;
            var dc = app.CreateController<DialogController>();
            dc.AcceptAction.Execute += AcceptAction_Execute;
            dc.Cancelling += dc_Cancelling;
            svp.Controllers.Add(dc);
        }

        private void AttachDialogController(ShowViewParameters svp, XafApplication app, MessageBoxEventHandler Accept) {
            localAccept = Accept;
            var dc = app.CreateController<DialogController>();
            dc.AcceptAction.Execute += AcceptAction_Execute;
            dc.CancelAction.Enabled.SetItemValue("Cancel Disabled", false);
            dc.CancelAction.Active.SetItemValue("Cancel Disabled", false);
            svp.Controllers.Add(dc);
        }

        private void AttachDialogController(ShowViewParameters svp, XafApplication app) {
            var dc = app.CreateController<DialogController>();
            dc.AcceptAction.Execute += AcceptAction_Execute;
            dc.CancelAction.Enabled.SetItemValue("Cancel Disabled", false);
            dc.CancelAction.Active.SetItemValue("Cancel Disabled", false);
            svp.Controllers.Add(dc);
        }

        private static void CreateDetailView(ShowViewParameters svp, XafApplication app, string Message) {
            svp.CreatedView = app.CreateDetailView(ObjectSpaceInMemory.CreateNew(), new MessageBoxTextMessage(Message)); ;
            svp.TargetWindow = TargetWindow.NewModalWindow;
            svp.Context = TemplateContext.PopupWindow;
            svp.CreateAllControllers = true;
        }

        void AcceptAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            if (localAccept != null)
                localAccept(this, e.ShowViewParameters);
        }

        void dc_Cancelling(object sender, EventArgs e) {
            if (localCancel != null)
                localCancel(this, e);
        }
    }
}
