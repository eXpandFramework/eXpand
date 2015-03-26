using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using ModelDifferenceTester.Module.FunctionalTests.WCModel;


namespace ModelDifferenceTester.Module {
    public sealed partial class ModelDifferenceTesterModule : ModuleBase {
        public ModelDifferenceTesterModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new[] { updater ,new WCModelUpdater(objectSpace, versionFromDB)};
        }
    }
}
