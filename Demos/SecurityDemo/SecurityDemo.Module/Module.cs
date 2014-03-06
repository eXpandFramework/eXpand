using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace SecurityDemo.Module {
    public sealed partial class SecurityDemoModule : ModuleBase {
        public SecurityDemoModule() {
            InitializeComponent();
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new[] { updater };
        }
    }
}
