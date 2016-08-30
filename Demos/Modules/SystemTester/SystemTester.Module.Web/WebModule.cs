using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.TreeListEditors.Web;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppModuleBasetopic.
    public sealed partial class SystemTesterAspNetModule : ModuleBase {

        public SystemTesterAspNetModule() {
            InitializeComponent();
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (Application.GetEasyTestParameter("ColumnChooser"))
                moduleManager.AddModule(Application,new XpandTreeListEditorsAspNetModule());
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
