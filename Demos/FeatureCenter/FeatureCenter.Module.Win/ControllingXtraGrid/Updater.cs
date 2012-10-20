using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using FeatureCenter.Module.BaseObjects;

namespace FeatureCenter.Module.Win.XtraGrid {
    public class Updater:ModuleUpdater {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            new CustomerBuilder<CXRCustomer>(Session).CreateCustomers();
            new CustomerBuilder<CXRCCustomer>(Session).CreateCustomers();
            new CustomerBuilder<GAFVCCustomer>(Session).CreateCustomers();
        }
    }
}