using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Validation.Web;
using DevExpress.Utils;
using Xpand.ExpressApp.Security.Web.AuthenticationProviders;
using Xpand.ExpressApp.Security.Web.Controllers;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security.Web {
    [ToolboxBitmap(typeof(SecurityModule), "Resources.BO_Security.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XpandSecurityWebModule : XpandSecurityModuleBase {
        private Authentication _authentication;

        public XpandSecurityWebModule() {
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
            RequiredModuleTypes.Add(typeof(ValidationAspNetModule));
        }

        protected override Type[] ApplicationTypes() {
            return new[] { typeof (IWriteSecuredLogonParameters) };
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (RuntimeMode){
                _authentication = new Authentication();
                _authentication.Attach(this);
                Application.CreateCustomLogonWindowControllers += application_CreateCustomLogonWindowControllers;
            }
        }

        private void application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e){
            e.Controllers.Add(new AnonymousLogonParamsController());
        }
    }
}