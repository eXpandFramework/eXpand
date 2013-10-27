using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Updater = EmailTester.Module.DatabaseUpdate.Updater;

namespace EmailTester.Module {
    public sealed partial class EmailTesterModule : ModuleBase {
        public EmailTesterModule() {
            InitializeComponent();
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new[]{updater};
        }
    }
}
