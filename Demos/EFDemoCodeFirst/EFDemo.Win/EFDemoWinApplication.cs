using System;
using System.Data.Common;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.EF;
using DevExpress.ExpressApp.Win;
using EFDemo.Module;
using EFDemo.Module.Data;

namespace EFDemo.Win {
	public partial class EFDemoWinApplication : WinApplication {
		private void EFDemoWinApplication_DatabaseVersionMismatch(Object sender, DatabaseVersionMismatchEventArgs e) {
			e.Updater.Update();
			e.Handled = true;
		}
		protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
			if(args.Connection != null) {
				args.ObjectSpaceProvider = new EFObjectSpaceProviderCF(typeof(EFDemoDbContext), (TypesInfo)TypesInfo, null, (DbConnection)args.Connection);
			}
			else {
				args.ObjectSpaceProvider = new EFObjectSpaceProviderCF(typeof(EFDemoDbContext), (TypesInfo)TypesInfo, null, args.ConnectionString);
			}
		}
		public EFDemoWinApplication() {
			InitializeComponent();
			DelayedViewItemsInitialization = true;
			DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderControllerBase.ScriptRecorderEnabled = true;
		}
	}
}
