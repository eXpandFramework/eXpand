using System;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.AuditTrail.Model.Member;

namespace AuditTrailTester.Module {
    public sealed partial class AuditTrailTesterModule : ModuleBase { // http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppModuleBasetopic
        public AuditTrailTesterModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new[] { updater };
        }

        public override void CustomizeLogics(DevExpress.ExpressApp.DC.CustomLogics customLogics) {
            base.CustomizeLogics(customLogics);
            customLogics.RegisterLogic(typeof(IModelMemberAuditTrail),typeof(ModelMemberAuditTrailDomainLogic));
        }
    }
}
