using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.RecursiveFiltering {
    public class Updater : ModuleUpdater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            if (ObjectSpace.Session.FindObject<RFCategory>(null) == null) {
                var goodCategory = new RFCategory(ObjectSpace.Session) { Name = "Good" };
                var veryGoodCategory = new RFCategory(ObjectSpace.Session) { Name = "Very Good", Parent = goodCategory };
                var bestCategory = new RFCategory(ObjectSpace.Session) { Name = "Best", Parent = veryGoodCategory };
                var badCategory = new RFCategory(ObjectSpace.Session) { Name = "Bad" };
                AssignCategory(goodCategory, veryGoodCategory, bestCategory, badCategory);
                ObjectSpace.CommitChanges();
            }
        }

        void AssignCategory(RFCategory goodRfCategory, RFCategory veryGoodRfCategory, RFCategory bestRfCategory, RFCategory badRfCategory) {
            var customers = new XPCollection<RFCustomer>(ObjectSpace.Session);
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
