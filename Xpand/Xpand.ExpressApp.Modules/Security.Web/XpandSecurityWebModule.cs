using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Utils;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Web.AuthenticationProviders;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security.Web {
    [ToolboxBitmap(typeof(SecurityModule), "Resources.BO_Security.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XpandSecurityWebModule : XpandSecurityModuleBase {
        public XpandSecurityWebModule() {
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsAuthentication>();
        }

        protected override void AddRegistrationControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            if (!((IModelOptionsAuthentication) Application.Model.Options).Athentication.AnonymousAuthentication.Enabled)
                base.AddRegistrationControllers(sender, e);
        }

        protected override Type[] ApplicationTypes() {
            return new[] { typeof (IWriteSecuredLogonParameters) };
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (RuntimeMode) {
                new Authentication().Attach(this);
            }
        }

        
    }
}