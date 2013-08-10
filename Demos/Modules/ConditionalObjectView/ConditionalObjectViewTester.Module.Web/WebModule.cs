using System;
using System.ComponentModel;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace ConditionalObjectViewTester.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class ConditionalObjectViewTesterAspNetModule : ModuleBase {
        public ConditionalObjectViewTesterAspNetModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
