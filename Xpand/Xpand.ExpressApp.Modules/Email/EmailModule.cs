using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Xpand.ExpressApp.Email.Logic;
using Xpand.ExpressApp.Email.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Validation;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Email {
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules), ToolboxBitmap(typeof (EmailModule)), ToolboxItem(true)]
    public sealed class EmailModule : XpandModuleBase{
        private IRazorEngineService _razorEngineService;
        public event EventHandler<RazorEngineArgs> CustomCreateRazorEngine;
        public EmailModule() {
            RequiredModuleTypes.Add(typeof (XpandValidationModule));
            LogicInstallerManager.RegisterInstaller(new EmailLogicInstaller(this));
        }

        public IRazorEngineService RazorEngineService{
            get { return _razorEngineService ?? (_razorEngineService = CreateRazorEngine()); }
        }

        void application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            if (((IModelApplicationEmail) Application.Model).Email.CreateControllersOnLogon) {
                e.Controllers.Add(Application.CreateController<LogicRuleViewController>());
                e.Controllers.Add(Application.CreateController<EmailRuleViewController>());
            }
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication, IModelApplicationEmail>();
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (Application != null){
                Application.CreateCustomLogonWindowControllers +=application_CreateCustomLogonWindowControllers;
                Application.UserDifferencesLoaded+=ApplicationOnUserDifferencesLoaded;
            }
        }

        private void ApplicationOnUserDifferencesLoaded(object sender, EventArgs eventArgs){
            CreateRazorEngine();
        }

        private IRazorEngineService CreateRazorEngine(){
            var razorEngineArgs = new RazorEngineArgs();
            OnCustomCreateRazorEngine(razorEngineArgs);
            if (razorEngineArgs.Engine != null){
                return razorEngineArgs.Engine;
            }
            var configuration = new TemplateServiceConfiguration{
                Debug = Debugger.IsAttached,
                DisableTempFileLocking = !Debugger.IsAttached
            };
            return RazorEngine.Templating.RazorEngineService.Create(configuration);
        }

        private void OnCustomCreateRazorEngine(RazorEngineArgs e){
            var handler = CustomCreateRazorEngine;
            if (handler != null) handler(this, e);
        }
    }

    public class RazorEngineArgs : EventArgs{
        public IRazorEngineService Engine { get; set; }
    }
}