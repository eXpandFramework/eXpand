using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Updater = SecurityTester.Module.Win.FunctionalTests.OverallCustomizationAllowed.Updater;

namespace SecurityTester.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class SecurityTesterWindowsFormsModule : ModuleBase {
        public SecurityTesterWindowsFormsModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return new[]{new Updater(objectSpace, versionFromDB) };
        }
    }
}
