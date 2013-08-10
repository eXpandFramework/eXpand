using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Updater = ConditionalObjectViewTester.Module.DatabaseUpdate.Updater;


namespace ConditionalObjectViewTester.Module {
    public sealed partial class ConditionalObjectViewTesterModule : ModuleBase {
        public ConditionalObjectViewTesterModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
    }
}
