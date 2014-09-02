using System;
using System.Data.Entity;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.Persistent.BaseImpl.EF;
using EFDemo.Module.Data;

namespace EFDemo.Module {
	public sealed partial class EFDemoModule : ModuleBase {
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return base.GetDeclaredExportedTypes();
		}
		static EFDemoModule() {
			Database.SetInitializer<EFDemoDbContext>(null);
			Person.FullNameFormat = "{LastName} {FirstName} {MiddleName}";
		}
		public EFDemoModule() {
			InitializeComponent();
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			((ViewVariantsModule)application.Modules.FindModule(typeof(ViewVariantsModule))).GenerateVariantsNode = false;
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
			return new ModuleUpdater[] { updater };
		}
	}
}
