using System;
using System.ComponentModel;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace ModelDifferenceTester.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class ModelDifferenceTesterAspNetModule : ModuleBase {
        public ModelDifferenceTesterAspNetModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
