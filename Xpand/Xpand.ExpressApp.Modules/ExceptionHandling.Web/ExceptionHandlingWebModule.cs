using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;

namespace Xpand.ExpressApp.ExceptionHandling.Web {
    [ToolboxBitmap(typeof(ExceptionHandlingWebModule))]
    [ToolboxItem(true)]
    public sealed partial class ExceptionHandlingWebModule : ExceptionHandlingModule {
        public ExceptionHandlingWebModule() {
            InitializeComponent();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            ErrorHandling.CustomSendErrorNotification +=ErrorHandlingOnCustomSendErrorNotification;
        }

        void ErrorHandlingOnCustomSendErrorNotification(object sender, CustomSendErrorNotificationEventArgs customSendErrorNotificationEventArgs) {
            Log(customSendErrorNotificationEventArgs.Exception);
        }


    }
}
