using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.ExpressApp.Updating;

namespace Xpand.Docs.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class DocsAspNetModule : ModuleBase {
        public DocsAspNetModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
