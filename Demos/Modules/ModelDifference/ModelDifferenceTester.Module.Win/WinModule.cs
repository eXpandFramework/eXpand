using System;
using System.Collections.Generic;
using System.ComponentModel;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.WorldCreator.Win;
using Xpand.Persistent.Base.General;

namespace ModelDifferenceTester.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class ModelDifferenceTesterWindowsFormsModule : ModuleBase {
        public ModelDifferenceTesterWindowsFormsModule() {
            InitializeComponent();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (application.GetEasyTestParameter("WCModel"))
                Application.Modules.Add(new WorldCreatorWinModule());
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
