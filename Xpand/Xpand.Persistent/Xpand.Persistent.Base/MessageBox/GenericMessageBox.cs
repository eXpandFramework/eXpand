using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.Persistent.Base.MessageBox {
    public class GenericMessageBox {

        public delegate void MessageBoxEventHandler(object sender, ShowViewParameters e);

        private event MessageBoxEventHandler LocalAccept;
        private event EventHandler LocalCancel;

        public GenericMessageBox(ShowViewParameters svp, XafApplication app, string message, MessageBoxEventHandler accept, EventHandler cancel) {
            CreateDetailView(svp, app, message);
            AttachDialogController(svp, app, accept, cancel);
        }

        public GenericMessageBox(ShowViewParameters svp, XafApplication app, string message, MessageBoxEventHandler accept) {
            CreateDetailView(svp, app, message);
            AttachDialogController(svp, app, accept);
        }

        public GenericMessageBox(ShowViewParameters svp, XafApplication app, string message) {
            CreateDetailView(svp, app, message);
            AttachDialogController(svp, app);
        }

        private void AttachDialogController(ShowViewParameters svp, XafApplication app, MessageBoxEventHandler accept, EventHandler cancel) {
            LocalAccept = accept;
            LocalCancel = cancel;
            var dc = app.CreateController<DialogController>();
            dc.AcceptAction.Execute += AcceptAction_Execute;
            dc.Cancelling += dc_Cancelling;
            svp.Controllers.Add(dc);
        }

        private void AttachDialogController(ShowViewParameters svp, XafApplication app, MessageBoxEventHandler accept) {
            LocalAccept = accept;
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

        private static void CreateDetailView(ShowViewParameters svp, XafApplication app, string message) {
            svp.CreatedView = app.CreateDetailView(ObjectSpaceInMemory.CreateNew(), new MessageBoxTextMessage(message));
            svp.TargetWindow = TargetWindow.NewModalWindow;
            svp.Context = TemplateContext.PopupWindow;
            svp.CreateAllControllers = true;
        }

        void AcceptAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            if (LocalAccept != null)
                LocalAccept(this, e.ShowViewParameters);
        }

        void dc_Cancelling(object sender, EventArgs e) {
            if (LocalCancel != null)
                LocalCancel(this, e);
        }
    }
}
