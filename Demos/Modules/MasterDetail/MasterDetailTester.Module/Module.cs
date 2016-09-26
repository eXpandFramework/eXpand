using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using Updater = MasterDetailTester.Module.DatabaseUpdate.Updater;

namespace MasterDetailTester.Module {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppModuleBasetopic.aspx.
    public sealed partial class MasterDetailTesterModule : ModuleBase {
        public MasterDetailTesterModule() {
            InitializeComponent();
			BaseObject.OidInitializationMode = OidInitializationMode.AfterConstruction;
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            // Manage various aspects of the application UI and behavior at the module level.
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }
    }
}
