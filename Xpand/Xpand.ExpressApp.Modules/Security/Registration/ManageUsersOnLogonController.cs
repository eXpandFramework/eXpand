using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using Fasterflect;

namespace Xpand.ExpressApp.Security.Registration {
    public class ManageUsersOnLogonController : ViewController<DetailView> {
        protected const string LogonActionParametersActiveKey = "Active for ILogonActionParameters only";
        public const string EmailPattern = @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$";
        private readonly SimpleAction _restorePassword;
        private readonly SimpleAction _registerUser;
        public ManageUsersOnLogonController() {
            _registerUser = CreateLogonSimpleAction("RegisterUser", "RegisterUserCategory", "Register User", "BO_User", "Register a new user within the system", typeof(RegisterUserParameters));
            _restorePassword = CreateLogonSimpleAction("RestorePassword", "RestorePasswordCategory", "Restore Password", "Action_ResetPassword", "Restore forgotten login information", typeof(RestorePasswordParameters));
        }
        
        protected override void OnViewChanging(View view) {
            base.OnViewChanging(view);
            Active[ControllerActiveKey] = !SecuritySystem.IsAuthenticated;
        }
        
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            bool flag = GetLogonParametersActiveState();
            foreach (Controller item in Frame.Controllers) {
                var logonController = item as LogonController;
                if (logonController != null) {
                    logonController.AcceptAction.Active[LogonActionParametersActiveKey] = !flag;
                    logonController.CancelAction.Active[LogonActionParametersActiveKey] = !flag;
                } else {
                    var dialogController = item as DialogController;
                    if (dialogController != null) {
                        dialogController.AcceptAction.Active[LogonActionParametersActiveKey] = flag;
                        dialogController.CancelAction.Active[LogonActionParametersActiveKey] = flag;
                        ConfigureDialogController(dialogController);
                    }
                }
            }
        }
        
        private SimpleAction CreateLogonSimpleAction(string id, string category, string caption, string imageName, string toolTip, Type parametersType) {
            var action = new SimpleAction(this, id, category) {
                Caption = caption,
                ImageName = imageName,
                PaintStyle = ActionItemPaintStyle.Image,
                ToolTip = toolTip
            };
            action.Execute += CreateParametersView;
            action.Tag = parametersType;
            return action;
        }
        
        private void CreateParametersView(object sender, SimpleActionExecuteEventArgs e) {

            Application.CallMethod("EnsureShowViewStrategy",Flags.InstancePrivate);
            CreateParametersViewCore(e);
        }
        
        protected virtual void CreateParametersViewCore(SimpleActionExecuteEventArgs e) {
            var parametersType = e.Action.Tag as Type;
            Guard.ArgumentNotNull(parametersType, "parametersType");
            if (parametersType != null) {
                var detailView = Application.CreateDetailView(Application.CreateObjectSpace(), parametersType.CreateInstance());
                detailView.ViewEditMode = ViewEditMode.Edit;
                e.ShowViewParameters.CreatedView = detailView;
            }

            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow = TargetWindow.Current;
        }

        protected virtual void ConfigureDialogController(DialogController dialogController) {
            dialogController.AcceptAction.Execute -= AcceptAction_Execute;
            dialogController.CancelAction.Execute -= CancelAction_Execute;
            dialogController.AcceptAction.Executing -= AcceptActionOnExecuting;
            dialogController.AcceptAction.Executing += AcceptActionOnExecuting;
            dialogController.AcceptAction.Execute += AcceptAction_Execute;
            dialogController.CancelAction.Execute += CancelAction_Execute;
            dialogController.SaveOnAccept = true;
            dialogController.Tag = typeof(ILogonRegistrationParameters);
        }

        void AcceptActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            ((DialogController) ((ActionBase) sender).Controller).Window.View.ObjectSpace.CommitChanges();
        }

        protected DialogController CreateDialogController() {
            var dialogController = Application.CreateController<DialogController>();
            ConfigureDialogController(dialogController);
            return dialogController;
        }
        
        private void AcceptAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            AcceptParameters(e.CurrentObject as ILogonRegistrationParameters);
        }
        
        private void CancelAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            CancelParameters(e.CurrentObject as ILogonRegistrationParameters);
        }
        protected virtual void AcceptParameters(ILogonRegistrationParameters parameters) {
            if (parameters != null)
                parameters.Process(Application.CreateObjectSpace());
            Application.LogOff();
        }
        protected virtual void CancelParameters(ILogonRegistrationParameters parameters) {
            Application.LogOff();
        }
        
        protected virtual bool GetLogonParametersActiveState() {
            return View != null && View.ObjectTypeInfo != null && View.ObjectTypeInfo.Implements<ILogonRegistrationParameters>();
        }
        public SimpleAction RestorePasswordAction {
            get { return _restorePassword; }
        }
        public SimpleAction RegisterUserAction {
            get { return _registerUser; }
        }
    }
}