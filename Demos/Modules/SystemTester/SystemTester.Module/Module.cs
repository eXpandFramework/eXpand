using System;
using SystemTester.Module.FunctionalTests.RuntimeMembers;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.ExpressApp.Updating;

namespace SystemTester.Module {
    public sealed partial class SystemTesterModule : ModuleBase {
        public SystemTesterModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new[] { updater,new RuntimeMembersUpdater(objectSpace, Version)  };
        }
    }
}
