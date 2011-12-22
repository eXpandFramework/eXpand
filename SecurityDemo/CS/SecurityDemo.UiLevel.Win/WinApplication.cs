using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Win;

namespace SecurityDemo.UiLevel.Win
{
    public partial class SecurityDemoWindowsFormsApplication : XpandWinApplication
    {
		public SecurityDemoWindowsFormsApplication()
        {
            InitializeComponent();
        }

		private void SecurityDemoWindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
            try {
                e.Updater.Update();
                e.Handled = true;
            }
            catch(CompatibilityException exception) {
                if(exception.Error is CompatibilityUnableToOpenDatabaseError) {
                    throw new UserFriendlyException(
                    "The connection to the database failed. This demo requires the local instance of Microsoft SQL Server Express. To use another database server,\r\nopen the demo solution in Visual Studio and modify connection string in the \"app.config\" file.");
                }
            }
        }
    }
}
