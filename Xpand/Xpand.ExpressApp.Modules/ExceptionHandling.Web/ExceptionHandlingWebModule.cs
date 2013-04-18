using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using DevExpress.Utils;

namespace Xpand.ExpressApp.ExceptionHandling.Web {
    [ToolboxBitmap(typeof(ExceptionHandlingWebModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabModules)]
    public sealed class ExceptionHandlingWebModule : ExceptionHandlingModule {

        public override void Setup(XafApplication application) {
            base.Setup(application);
            ErrorHandling.CustomSendErrorNotification += ErrorHandlingOnCustomSendErrorNotification;
        }

        void ErrorHandlingOnCustomSendErrorNotification(object sender, CustomSendErrorNotificationEventArgs customSendErrorNotificationEventArgs) {
            Log(customSendErrorNotificationEventArgs.Exception);
        }


    }
}
