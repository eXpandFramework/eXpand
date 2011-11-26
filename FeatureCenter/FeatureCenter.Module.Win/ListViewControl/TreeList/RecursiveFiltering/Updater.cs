using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.RecursiveFiltering {
    public class Updater : FCUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion, Xpand.Persistent.BaseImpl.Updater updater)
            : base(objectSpace, currentDBVersion,updater) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            var session = ((ObjectSpace)ObjectSpace).Session;
            if (session.FindObject<RFCategory>(null) == null) {
                var goodCategory = new RFCategory(session) { Name = "Good" };
                var veryGoodCategory = new RFCategory(session) { Name = "Very Good", Parent = goodCategory };
                var bestCategory = new RFCategory(session) { Name = "Best", Parent = veryGoodCategory };
                var badCategory = new RFCategory(session) { Name = "Bad" };
                AssignCategory(goodCategory, veryGoodCategory, bestCategory, badCategory);
                ObjectSpace.CommitChanges();
            }
        }

        void AssignCategory(RFCategory goodRfCategory, RFCategory veryGoodRfCategory, RFCategory bestRfCategory, RFCategory badRfCategory) {
            var session = ((ObjectSpace)ObjectSpace).Session;
            var customers = new XPCollection<RFCustomer>(session);
            for (int i = 0; i < customers.Count; i++) {
                switch (i) {
                    case 0:
                        customers[i].Category = goodRfCategory;
                        break;
                    case 1:
                        customers[i].Category = veryGoodRfCategory;
                        break;
                    case 2:
                        customers[i].Category = bestRfCategory;
                        break;
                    case 3:
                        customers[i].Category = badRfCategory;
                        break;
                    case 4:
                        customers[i].Category = goodRfCategory;
                        break;
                    case 5:
                        customers[i].Category = veryGoodRfCategory;
                        break;
                    case 6:
                        customers[i].Category = bestRfCategory;
                        break;
                }
            }
        }
    }
}
