using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using FeatureCenter.Module;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win;
using Xpand.ExpressApp.Core;
using Application = System.Windows.Forms.Application;

namespace FeatureCenter.Win {
    public partial class FeatureCenterWindowsFormsApplication : XpandWinApplication {
        public FeatureCenterWindowsFormsApplication() {
            InitializeComponent();
        }

        //        protected override ShowViewStrategyBase CreateShowViewStrategy() {
        //            return new ShowInSingleWindowStrategy(this);
        //        }

        private void FeatureCenterWindowsFormsApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {

#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            if (true) {
                if (this.DropDatabaseOnVersionMissmatch() > 0)
                    Application.ExitThread();
                e.Updater.Update();
                e.Handled = true;
            } else {
                throw new InvalidOperationException(
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "The automatic update is disabled, because the application was started without debugging.\r\n" +
                    "You should start the application under Visual Studio, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.");
            }
#endif
        }
    }
}
