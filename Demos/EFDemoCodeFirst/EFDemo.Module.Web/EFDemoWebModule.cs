using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace EFDemo.Module.Web {
	[ToolboxItemFilter("Xaf.Platform.Web")]
	public sealed partial class EFDemoWebModule : ModuleBase {
		public EFDemoWebModule() {
			InitializeComponent();
		}
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
    }
}
