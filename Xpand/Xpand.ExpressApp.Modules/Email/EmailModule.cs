using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.Email.Logic;
using Xpand.ExpressApp.Email.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Validation;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Email {
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules), ToolboxBitmap(typeof (EmailModule)), ToolboxItem(true)]
    public sealed class EmailModule : XpandModuleBase {
        public EmailModule() {
            RequiredModuleTypes.Add(typeof (XpandValidationModule));
            LogicInstallerManager.RegisterInstaller(new EmailLogicInstaller(this));
        }

        void application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            if (((IModelApplicationEmail) Application.Model).CreateControllersOnLogon) {
                e.Controllers.Add(Application.CreateController<LogicRuleViewController>());
                e.Controllers.Add(Application.CreateController<EmailRuleViewController>());
            }
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            ((XafApplication) sender).CreateCustomLogonWindowControllers +=
                application_CreateCustomLogonWindowControllers;
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication, IModelApplicationEmail>();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete += ApplicationOnSetupComplete;
        }
    }
}