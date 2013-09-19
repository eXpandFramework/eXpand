using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using MapViewTester.Module.BusinessObjects;

namespace MapViewTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<Customer>(null) == null) {
                var customer = ObjectSpace.CreateObject<Customer>();
                customer.FirstName = "Apostolis";
                customer.LastName = "Bekiaris";
                customer.Address1=ObjectSpace.CreateObject<Address>();
                customer.Address1.City = "Athens";
                var country = ObjectSpace.CreateObject<Country>();
                country.Name = "Greece";
                customer.Address1.Country = country;
                
                customer = ObjectSpace.CreateObject<Customer>();
                customer.FirstName = "Sergej";
                customer.LastName = "Derjabkin";
                customer.Address1 = ObjectSpace.CreateObject<Address>();
                customer.Address1.City = "Dortmund";
                country = ObjectSpace.CreateObject<Country>();
                country.Name = "Germany";
                customer.Address1.Country = country;

                ObjectSpace.CommitChanges();
            }
        }
    }
}
