﻿using System;
using System.ComponentModel;
using System.Web;
using System.Web.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Registration;
using Xpand.ExpressApp.Security.Web.AuthenticationProviders;

namespace Xpand.ExpressApp.Security.Web.Controllers {
    public class AnonymousLogonController : Security.Controllers.AnonymousLogonController {
        string _userName;
        int _loginAttempts;

        public AnonymousLogonController() {
            var popupWindowShowAction = new PopupWindowShowAction(this, "LogonAnonymous", "Security") { Caption = "Login" };
            popupWindowShowAction.Execute+=PopupWindowShowActionOnExecute;
            popupWindowShowAction.CustomizePopupWindowParams+=PopupWindowShowActionOnCustomizePopupWindowParams;
        }

        void PopupWindowShowActionOnExecute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            var anonymousLogonParameters = e.PopupWindow.View.CurrentObject as AnonymousLogonParameters;
            if (anonymousLogonParameters!=null&&HttpContext.Current != null && !(_loginAttempts >= 3)) {
                _loginAttempts++;
                try {
                    ((SecurityStrategyBase) Application.Security).Authentication.Authenticate(e.PopupWindow.View.ObjectSpace);
                }
                catch (AuthenticationException) {
                    if (_loginAttempts >= 3) {
                        anonymousLogonParameters.UserName = _userName;
                        e.PopupWindow.SetView(LogonAttemptsAmountedToLimitDetailView());
                        e.CanCloseWindow = false;
                        return;
                    }
                    throw;
                }
                LoginAnonymously(anonymousLogonParameters);
            }

        }

        DetailView LogonAttemptsAmountedToLimitDetailView() {
            return Application.CreateDetailView(new NonPersistentObjectSpace(Application.TypesInfo),ModelNodeIdHelper.GetDetailViewId(typeof (LogonAttemptsAmountedToLimit)), true);
        }

        void LoginAnonymously(AnonymousLogonParameters anonymousLogonParameters) {
            anonymousLogonParameters.AnonymousLogin = false;
            ObjectSerializer.WriteObjectPropertyValues(null, anonymousLogonParameters.Storage, anonymousLogonParameters);
            SecuritySystem.Instance.Logoff();
            FormsAuthentication.SignOut();
            HttpContext.Current.Session.Abandon();
        }

        void PopupWindowShowActionOnCustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs args) {
            var objectSpace = Application.CreateObjectSpace();
            var anonymousLogonParameters = ((AnonymousLogonParameters) Application.Security.LogonParameters);
            _userName = anonymousLogonParameters.UserName;
            anonymousLogonParameters.UserName = null;
            var detailView = Application.CreateDetailView(objectSpace, anonymousLogonParameters);
            detailView.ViewEditMode = ViewEditMode.Edit;
            args.View=_loginAttempts>=3?LogonAttemptsAmountedToLimitDetailView():detailView;
            var registrationControllers = XpandSecurityModuleBase.CreateRegistrationControllers(Application);
            args.DialogController.Controllers.AddRange(registrationControllers);
            args.DialogController.SaveOnAccept = false;
            args.DialogController.Activated+=DialogControllerOnActivated;
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
