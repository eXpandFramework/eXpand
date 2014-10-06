using System;
using SystemTester.Module.FunctionalTests.RuntimeMembers;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.ExpressApp.Updating;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module {
    public sealed partial class SystemTesterModule : EasyTestModule {
        public SystemTesterModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new[] { updater,new RuntimeMembersUpdater(objectSpace, Version)  };
        }
    }
}
