using System;
using System.Collections.Generic;
using System.ComponentModel;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Updater = MasterDetailTester.Module.Win.DatabaseUpdate.Updater;

namespace MasterDetailTester.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class MasterDetailTesterWindowsFormsModule : ModuleBase {
        public MasterDetailTesterWindowsFormsModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return new[] { new Updater(objectSpace, versionFromDB) };
        }
    }
}
