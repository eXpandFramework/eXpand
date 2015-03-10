using System;
using System.ComponentModel;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using SecurityTester.Module.Web.FunctionalTests.Anonymous;

namespace SecurityTester.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class SecurityTesterAspNetModule : ModuleBase {
        public SecurityTesterAspNetModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return new ModuleUpdater[]{new Updater(objectSpace, Version), new AnonymousRoleUpdater(objectSpace, Version)};
        }
    }
}
