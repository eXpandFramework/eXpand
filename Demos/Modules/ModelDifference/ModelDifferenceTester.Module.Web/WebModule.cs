using System;
using System.ComponentModel;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using ModelDifferenceTester.Module.FunctionalTests;
using Xpand.ExpressApp.WorldCreator.Web;
using Xpand.Persistent.Base.General;

namespace ModelDifferenceTester.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class ModelDifferenceTesterAspNetModule : ModuleBase {
        public ModelDifferenceTesterAspNetModule() {
            InitializeComponent();
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (Application != null && Application.GetEasyTestParameter(EasyTestParameters.WCModel))
                moduleManager.AddModule(Application, new WorldCreatorWebModule());
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
