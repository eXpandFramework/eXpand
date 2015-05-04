using System;
using SystemTester.Module.FunctionalTests.RuntimeMembers;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Security.Controllers;
using Xpand.Persistent.Base.General;

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
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new[] { updater,new RuntimeMembersUpdater(objectSpace, Version)  };
        }
    }
}
