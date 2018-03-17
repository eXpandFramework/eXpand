using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Web;

namespace ExcelImporterTester.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppModuleBasetopic.aspx.
    public sealed partial class ExcelImporterTesterAspNetModule : ModuleBase {
        public ExcelImporterTesterAspNetModule() {
            InitializeComponent();
            
        }


        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete+=ApplicationOnSetupComplete;
            // Manage various aspects of the application UI and behavior at the module level.
        }

        private void ApplicationOnSetupComplete(object sender, EventArgs eventArgs){
            
        }
    }
}
