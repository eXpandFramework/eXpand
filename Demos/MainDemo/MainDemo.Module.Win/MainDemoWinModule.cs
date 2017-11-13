using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.Persistent.Base.General;

namespace MainDemo.Module.Win {
	[ToolboxItemFilter("Xaf.Platform.Win")]
	public sealed partial class MainDemoWinModule : ModuleBase {
		public MainDemoWinModule() {
			InitializeComponent();
			DevExpress.ExpressApp.Scheduler.Win.SchedulerListEditor.DailyPrintStyleCalendarHeaderVisible = false;
            DevExpress.Persistent.Base.ReportsV2.DataSourceBase.EnableAsyncLoading = false;
            DevExpress.ExpressApp.ReportsV2.Win.WinReportServiceController.UseNewWizard = true;
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new[] { updater };

        }

	    public override void Setup(ApplicationModulesManager moduleManager) {
	        base.Setup(moduleManager);
//	        var directoryName = Path.GetDirectoryName(new Uri(GetType().Assembly.Location + "").LocalPath);
//	        var xpandDllPath = MainDemoModule.GetXpandDllPath(directoryName);
//	        this.AddModules(xpandDllPath);
	    }
    }
}
