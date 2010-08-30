using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using FeatureCenter.Module.BaseObjects;

namespace FeatureCenter.Module.Win.MasterDetail.ConditionalDetailViews {
    public class Updater:ModuleUpdater {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            new CustomerOrderOrderLineBuilder<MDCDVCustomer, MDCDVOrder, MDCDVOrderLine>(Session).CreateObjects();
        }
    }
}