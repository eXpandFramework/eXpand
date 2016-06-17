using System;
using System.ComponentModel;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.WorldCreator.Web;
using Xpand.Persistent.Base.General;

namespace ModelDifferenceTester.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class ModelDifferenceTesterAspNetModule : ModuleBase {
        public ModelDifferenceTesterAspNetModule() {
            InitializeComponent();
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            if (application.GetEasyTestParameter("WCModel"))
                Application.Modules.Add(new WorldCreatorWebModule());
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
