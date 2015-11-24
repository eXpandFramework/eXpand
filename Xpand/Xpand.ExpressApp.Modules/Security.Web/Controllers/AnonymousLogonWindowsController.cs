using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Registration;
using Xpand.ExpressApp.Security.Web.AuthenticationProviders;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security.Web.Controllers {
    public class AnonymousLogonParamsController : ObjectViewController<DetailView, AnonymousLogonParameters> {
        protected override void OnActivated(){
            base.OnActivated();
            var cancelAction = Frame.GetController<WebLogonController>().CancelAction;
            cancelAction.ActivateKey("Web application logon");
            cancelAction.Execute+=CancelActionOnExecute;
        }

        private void CancelActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            ((SimpleAction) sender).Execute-=CancelActionOnExecute;
            Application.LogOff();
        }
    }

    public class AnonymousLogonController : Security.Controllers.AnonymousLogonController {
        public AnonymousLogonController() {
            var popupWindowShowAction = new SimpleAction(this, "LogonAnonymous", "Security") { Caption = "Login" };
            popupWindowShowAction.Execute+=PopupWindowShowActionOnExecute;
        }

        private void PopupWindowShowActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            var anonymousLogonParameters = (AnonymousLogonParameters)SecuritySystem.LogonParameters;
            anonymousLogonParameters.AnonymousLogin = false;
            ObjectSerializer.WriteObjectPropertyValues(null, anonymousLogonParameters.Storage, anonymousLogonParameters);
            Application.LogOff();            
        }

        void DialogControllerOnActivated(object sender, EventArgs eventArgs) {
            var dialogController = ((DialogController) sender);
            dialogController.Activated-=DialogControllerOnActivated;
            dialogController.Deactivated+=DialogControllerOnDeactivated;
            var manageUsersOnLogonController = dialogController.Frame.GetController<ManageUsersOnLogonController>();
            manageUsersOnLogonController.CustomActiveKey += OnCustomActiveKey;
            manageUsersOnLogonController.CustomProccessedLogonParameter+=ManageUsersOnLogonControllerOnCustomProccessedLogonParameter;
            manageUsersOnLogonController.CustomCancelLogonParameter += ManageUsersOnLogonControllerOnCustomCancelLogonParameter;
        }

        void DialogControllerOnDeactivated(object sender, EventArgs eventArgs) {
            var dialogController = ((DialogController) sender);
            dialogController.Activated -= DialogControllerOnActivated;
            dialogController.Deactivated -= DialogControllerOnDeactivated;
            var manageUsersOnLogonController = dialogController.Frame.GetController<ManageUsersOnLogonController>();
            manageUsersOnLogonController.CustomActiveKey -= OnCustomActiveKey;
            manageUsersOnLogonController.CustomProccessedLogonParameter -= ManageUsersOnLogonControllerOnCustomProccessedLogonParameter;
            manageUsersOnLogonController.CustomCancelLogonParameter -= ManageUsersOnLogonControllerOnCustomCancelLogonParameter;
        }

        private void ManageUsersOnLogonControllerOnCustomCancelLogonParameter(object sender, ParameterEventArgs parameterEventArgs){
            parameterEventArgs.Handled=((IModelOptionsAuthentication) Application.Model.Options).Athentication.AnonymousAuthentication.Enabled;
        }

        private void ManageUsersOnLogonControllerOnCustomProccessedLogonParameter(object sender, HandledEventArgs handledEventArgs){
            handledEventArgs.Handled =((IModelOptionsAuthentication) Application.Model.Options).Athentication.AnonymousAuthentication.Enabled;
        }

        void OnCustomActiveKey(object sender, CustomActiveKeyArgs e) {
            e.Handled = e.View.ObjectTypeInfo.Type == typeof(AnonymousLogonParameters);
        }
    }
}
