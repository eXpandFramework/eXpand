using System;
using System.Data.Entity;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.EF;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.ViewVariantsModule;
using EFDemo.Module.Data;
using Updater = EFDemo.Module.DatabaseUpdate.Updater;

namespace EFDemo.Module {
	public sealed partial class EFDemoModule : ModuleBase {

	    static EFDemoModule() {
			Database.SetInitializer<EFDemoDbContext>(null);
			Person.FullNameFormat = "{LastName} {FirstName} {MiddleName}";
			ExportedTypeHelpers.AddExportedTypeHelper(new EFExportedTypeHelperCF());
            
		}
		public EFDemoModule() {
			InitializeComponent();
            
		}

		public override void Setup(XafApplication application) {
			base.Setup(application);
            
			((ViewVariantsModule)application.Modules.FindModule(typeof(ViewVariantsModule))).GenerateVariantsNode = false;
		}

	    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
	        IObjectSpace space = Application.ObjectSpaceProviders[1].CreateUpdatingObjectSpace(true);
            ModuleUpdater updater = new DatabaseUpdate.Updater(space, versionFromDB);
			return new ModuleUpdater[] { updater };
		}
	}
}
