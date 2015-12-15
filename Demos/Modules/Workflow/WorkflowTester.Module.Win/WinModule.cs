using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.WorldCreator.Win;
using Xpand.Persistent.Base.General;

namespace WorkflowTester.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class WorkflowTesterWindowsFormsModule : ModuleBase {
        public WorkflowTesterWindowsFormsModule() {
            InitializeComponent();
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (Application!=null&&Application.GetEasyTestParameter("WorldCreator")){
                moduleManager.AddModule(Application, (ModuleBase)Activator.CreateInstance(typeof(WorldCreatorWinModule)));
            }    
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
