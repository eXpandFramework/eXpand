using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.WorldCreator.Win;
using Xpand.Persistent.Base.General;

namespace IOTester.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class IOTesterWindowsFormsModule : ModuleBase {
        public IOTesterWindowsFormsModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (Application != null && Application.GetEasyTestParameter("NorthWind"))
                moduleManager.AddModule(Application, new WorldCreatorWinModule());
        }
    }
}
