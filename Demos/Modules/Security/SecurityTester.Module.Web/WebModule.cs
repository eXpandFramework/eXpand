using System;
using System.ComponentModel;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using SecurityTester.Module.Web.Controllers;
using SecurityTester.Module.Web.FunctionalTests.Anonymous;

namespace SecurityTester.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class SecurityTesterAspNetModule : ModuleBase {
        public SecurityTesterAspNetModule() {
            InitializeComponent();
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            application.CreateCustomLogonWindowControllers+=ApplicationOnCreateCustomLogonWindowControllers;
        }

        private void ApplicationOnCreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e){
            e.Controllers.Add(new LayoutStyleController());
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return new ModuleUpdater[]{new Updater(objectSpace, Version), new AnonymousRoleUpdater(objectSpace, Version)};
        }
    }
}
