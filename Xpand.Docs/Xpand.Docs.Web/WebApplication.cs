//using DevExpress.ExpressApp.Security;
using System;
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using Xpand.Docs.Module;
using Xpand.Docs.Module.Web;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Web.AuthenticationProviders;
using Xpand.ExpressApp.Web;

namespace Xpand.Docs.Web{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/DevExpressExpressAppWebWebApplicationMembersTopicAll
    public class DocsAspNetApplication : XpandWebApplication{
        private SystemModule _module1;
        private SystemAspNetModule _module2;
        private DocsModule _module3;
        private DocsAspNetModule _module4;

        public DocsAspNetApplication(){
            InitializeComponent();
        }

        protected override void OnSetupStarted(){
            base.OnSetupStarted();
            this.NewSecurityStrategyComplex<AnonymousAuthenticationStandard, AnonymousLogonParameters>();
        }

        protected override bool SupportMasterDetailMode{
            get { return true; }
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection, true);
        }

        private void DocsAspNetApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e){
#if EASYTEST
            e.Updater.Update();
            e.Handled = true;
#else
            if (true){
                e.Updater.Update();
                e.Handled = true;
            }
            else{
                string message =
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "This error occurred  because the automatic database update was disabled when the application was started without debugging.\r\n" +
                    "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.\r\n" +
                    "Anyway, refer to the following help topics for more detailed information:\r\n" +
                    "'Update Application and Database Versions' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm\r\n" +
                    "'Database Security References' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument3237.htm\r\n" +
                    "If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/";

                if (e.CompatibilityError != null && e.CompatibilityError.Exception != null){
                    message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
                }
                throw new InvalidOperationException(message);
            }
#endif
        }

        private void InitializeComponent(){
            _module1 = new SystemModule();
            _module2 = new SystemAspNetModule();
            _module3 = new DocsModule();
            _module4 = new DocsAspNetModule();
            ((ISupportInitialize) (this)).BeginInit();
            // 
            // DocsAspNetApplication
            // 
            ApplicationName = "Xpand.Docs";
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_module3);
            Modules.Add(_module4);

            DatabaseVersionMismatch += DocsAspNetApplication_DatabaseVersionMismatch;
            ((ISupportInitialize) (this)).EndInit();
        }
    }
}