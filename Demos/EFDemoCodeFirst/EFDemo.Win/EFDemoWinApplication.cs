using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using EFDemo.Module;
using Xpand.ExpressApp.Win;

namespace EFDemo.Win {
	public partial class EFDemoWinApplication : XpandWinApplication {
		private void EFDemoWinApplication_DatabaseVersionMismatch(Object sender, DatabaseVersionMismatchEventArgs e) {
			e.Updater.Update();
			e.Handled = true;
		}

	    protected override void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            this.CreateCustomProvider(args.ConnectionString, (TypesInfo) TypesInfo, args.ObjectSpaceProviders, () => base.OnCreateCustomObjectSpaceProvider(args));
	    }

		public EFDemoWinApplication() {
			InitializeComponent();
			DelayedViewItemsInitialization = true;
			DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderControllerBase.ScriptRecorderEnabled = true;
		}
	}

}
