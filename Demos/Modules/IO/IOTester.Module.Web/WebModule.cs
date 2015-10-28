using System;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.WorldCreator.Web;
using Xpand.Persistent.Base.General;

namespace IOTester.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class IOTesterAspNetModule : ModuleBase {
        public IOTesterAspNetModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (Application != null && Application.GetEasyTestParameter("NorthWind"))
                moduleManager.AddModule(new WorldCreatorWebModule());
        }
    }
}
