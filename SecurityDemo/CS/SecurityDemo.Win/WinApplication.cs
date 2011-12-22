using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Win;

namespace SecurityDemo.Win
{
    public partial class SecurityDemoWindowsFormsApplication : XpandWinApplication
    {
		public SecurityDemoWindowsFormsApplication()
        {
            InitializeComponent();
            
        }
        protected override void CreateXpandObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            
        }

    }
}
