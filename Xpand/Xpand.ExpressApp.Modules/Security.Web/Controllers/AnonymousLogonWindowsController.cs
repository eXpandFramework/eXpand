using System.Web;
using System.Web.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Security.Web.AuthenticationProviders;

namespace Xpand.ExpressApp.Security.Web.Controllers {
    public class AnonymousLogonWindowsController : Security.Controllers.AnonymousLogonWindowsController {
        string _userName;
        int _loginAttempts;

        public AnonymousLogonWindowsController() {
            var popupWindowShowAction = new PopupWindowShowAction(this, "LogonAnonymous", "Security") { Caption = "Login" };
            popupWindowShowAction.Execute+=PopupWindowShowActionOnExecute;
            popupWindowShowAction.CustomizePopupWindowParams+=PopupWindowShowActionOnCustomizePopupWindowParams;
        }

        void PopupWindowShowActionOnExecute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            if (HttpContext.Current != null && !(_loginAttempts >= 3)) {
                _loginAttempts++;
                var anonymousLogonParameters = (AnonymousLogonParameters)e.PopupWindow.View.CurrentObject;
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
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
        }

        void PopupWindowShowActionOnCustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs customizePopupWindowParamsEventArgs) {
            var objectSpace = Application.CreateObjectSpace();
            var anonymousLogonParameters = ((AnonymousLogonParameters) Application.Security.LogonParameters);
            _userName = anonymousLogonParameters.UserName;
            anonymousLogonParameters.UserName = null;
            var detailView = Application.CreateDetailView(objectSpace, anonymousLogonParameters);
            detailView.ViewEditMode = ViewEditMode.Edit;
            customizePopupWindowParamsEventArgs.View=_loginAttempts>=3?LogonAttemptsAmountedToLimitDetailView():detailView;
        }

    }
}
