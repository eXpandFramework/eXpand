using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using MasterDetailTester.Module.BusinessObjects;

namespace MasterDetailTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<Customer>(null) == null) {
                var customer = ObjectSpace.CreateObject<Customer>();
                customer.Name = "Apostolis Bekiaris";
                var order = ObjectSpace.CreateObject<Order>();
                order.Quantity = 2;
                customer.Orders.Add(order);
                ObjectSpace.CommitChanges();
            }
        }
    }
}
