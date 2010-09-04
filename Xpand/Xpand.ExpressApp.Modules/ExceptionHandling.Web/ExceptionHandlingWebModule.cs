using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;

namespace Xpand.ExpressApp.ExceptionHandling.Web
{
    public sealed partial class ExceptionHandlingWebModule : ExceptionHandlingModule
    {
        public ExceptionHandlingWebModule()
        {
            InitializeComponent();
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            ErrorHandling.CustomSendMailMessage+=ErrorHandlingOnCustomSendMailMessage;
        }

        private void ErrorHandlingOnCustomSendMailMessage(object sender, CustomSendMailMessageEventArgs args)
        {
            Log(HttpContext.Current.Server.GetLastError().GetBaseException());
        }

    }
}
