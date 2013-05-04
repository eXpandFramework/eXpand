using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.ModelDifference.Security;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace ExternalApplication.Module.Win {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public static void InitializeSecurity(IObjectSpace objectSpace) {
            var defaultRole = objectSpace.GetDefaultRole();
            var administratorRole = objectSpace.GetAdminRole("Administrator");
            var modelRole = objectSpace.GetDefaultModelRole("ModelDifference");

            objectSpace.GetUser("Admin", "Admin", administratorRole);
            objectSpace.GetUser("User", "", defaultRole, modelRole);

            objectSpace.CommitChanges();
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            InitializeSecurity(ObjectSpace);
            var space = (XPObjectSpace)ObjectSpace;
            new DummyDataBuilder(space).CreateObjects();
            if (space.Session.FindObject<PersistentAssemblyInfo>(CriteriaOperator.Parse("Name=?", "TestAssembly")) == null) {
                new PersistentAssemblyInfo(space.Session) { Name = "TestAssembly" };
                space.CommitChanges();
            }
        }
    }
}
