using System;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.ExpressApp.Updating;

namespace Xpand.Docs.Module {
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppModuleBasetopic.
    public sealed partial class DocsModule : ModuleBase {
        public DocsModule() {
            InitializeComponent();
        }


        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new[] { updater };
        }
    }
}
