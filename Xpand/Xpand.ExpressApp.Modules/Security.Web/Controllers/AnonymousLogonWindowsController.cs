using System.Web;
using System.Web.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using Xpand.ExpressApp.Security.Web.AuthenticationProviders;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security.Web.Controllers {
    public class AnonymousLogonWindowsController : Security.Controllers.AnonymousLogonWindowsController {
        public AnonymousLogonWindowsController() {
            var popupWindowShowAction = new PopupWindowShowAction(this, "LogonAnonymous", "Security") { Caption = "Login" };
            popupWindowShowAction.Execute+=PopupWindowShowActionOnExecute;
            popupWindowShowAction.CustomizePopupWindowParams+=PopupWindowShowActionOnCustomizePopupWindowParams;
        }

        void PopupWindowShowActionOnExecute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            if (HttpContext.Current != null) {
                SettingsStorage settingsStorage = ((ISettingsStorage) Application).CreateLogonParameterStoreCore();
                ObjectSerializer.WriteObjectPropertyValues(null, settingsStorage,e.PopupWindow.View.CurrentObject);

                SecuritySystem.Instance.Logoff();
                HttpContext.Current.Session.Abandon();
                FormsAuthentication.SignOut();
            }

        }

        void PopupWindowShowActionOnCustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs customizePopupWindowParamsEventArgs) {
            var objectSpace = Application.CreateObjectSpace();
            var detailView = Application.CreateDetailView(objectSpace, new AnonymousLogonParameters { AnonymousLogin = false});
            detailView.ViewEditMode = ViewEditMode.Edit;
            customizePopupWindowParamsEventArgs.View=detailView;
        }

    }
}
