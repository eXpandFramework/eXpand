using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using WorldCreatorTester.Module.BusinessObjects;

namespace WorldCreatorTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<Customer>(null) == null) {
                var customer = ObjectSpace.CreateObject<Customer>();
                customer.FirstName = "Apostolis";
                customer.LastName = "Bekiaris";
                ObjectSpace.CommitChanges();
            }
        }
    }
}
