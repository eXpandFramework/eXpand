using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Win;

namespace Solution3.Win
{
    public partial class Solution3WindowsFormsApplication : WinComponent
    {
        public Solution3WindowsFormsApplication()
        {
            InitializeComponent();
        }

        private void Solution3WindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e)
        {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            e.Updater.Update();
            e.Handled = true;
//            if (System.Diagnostics.Debugger.IsAttached)
//            {
//                e.Updater.Update();
//                e.Handled = true;
//            }
//            else
//            {
//                throw new InvalidOperationException(
//                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
//                    "The automatic update is disabled, because the application was started without debugging.\r\n" +
//                    "You should start the application under Visual Studio, or modify the " +
//                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
//                    "or manually create a database using the 'DBUpdater' tool.");
//            }
#endif
        }
    }
}
