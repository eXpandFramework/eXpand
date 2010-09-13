using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.ListViewControl.RecursiveFiltering
{
    public class Updater:ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            if (Session.FindObject<RFCategory>(null)== null) {
                var goodCategory = new RFCategory(Session){Name = "Good"};
                goodCategory.Save();
                var veryGoodCategory = new RFCategory(Session) { Name = "Very Good",Parent = goodCategory};
                veryGoodCategory.Save();
                var bestCategory = new RFCategory(Session){Name = "Best",Parent = veryGoodCategory};
                bestCategory.Save();
                var badCategory = new RFCategory(Session){Name = "Bad"};
                badCategory.Save();
                AssignCategory(goodCategory, veryGoodCategory, bestCategory, badCategory);
            }
        }

        void AssignCategory(RFCategory goodRfCategory, RFCategory veryGoodRfCategory, RFCategory bestRfCategory, RFCategory badRfCategory) {
            var customers = new XPCollection<RFCustomer>(Session);
            for (int i = 0; i < customers.Count; i++) {
                switch (i) {
                    case 0:
                        customers[i].RfCategory=goodRfCategory;
                        break;
                    case 1:
                        customers[i].RfCategory=veryGoodRfCategory;
                        break;
                    case 2:
                        customers[i].RfCategory=bestRfCategory;
                        break;
                    case 3:
                        customers[i].RfCategory=badRfCategory;
                        break;
                    case 4:
                        customers[i].RfCategory=goodRfCategory;
                        break;
                    case 5:
                        customers[i].RfCategory=veryGoodRfCategory;
                        break;
                    case 6:
                        customers[i].RfCategory=bestRfCategory;
                        break;
                }
                customers[i].Save();
            }
        }
    }
}
