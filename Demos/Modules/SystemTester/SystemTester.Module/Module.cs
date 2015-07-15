using System;
using System.Collections.Generic;
using SystemTester.Module.FunctionalTests.RuntimeMembers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Security.Controllers;
using Xpand.Persistent.Base.General;
using Updater = SystemTester.Module.DatabaseUpdate.Updater;

namespace SystemTester.Module {
    public sealed partial class SystemTesterModule : EasyTestModule {
        public SystemTesterModule() {
            InitializeComponent();
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            application.CreateCustomLogonWindowControllers+=ApplicationOnCreateCustomLogonWindowControllers;
        }

        private void ApplicationOnCreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e){
            e.Controllers.Add(new ChooseDatabaseAtLogonController());
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new[] { updater,new RuntimeMembersUpdater(objectSpace, Version)  };
        }
    }
}
