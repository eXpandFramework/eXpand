using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.Miscellaneous.RecursiveFiltering
{
    public class Updater:ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            if (Session.FindObject<Category>(null)== null) {
                var goodCategory = new Category(Session){Name = "Good"};
                goodCategory.Save();
                var veryGoodCategory = new Category(Session) { Name = "Very Good",Parent = goodCategory};
                veryGoodCategory.Save();
                var bestCategory = new Category(Session){Name = "Best",Parent = veryGoodCategory};
                bestCategory.Save();
                var badCategory = new Category(Session){Name = "Bad"};
                badCategory.Save();
                AssignCategory(goodCategory, veryGoodCategory, bestCategory, badCategory);
            }
        }

        void AssignCategory(Category goodCategory, Category veryGoodCategory, Category bestCategory, Category badCategory) {
            var customers = new XPCollection<RFCustomer>(Session);
            for (int i = 0; i < customers.Count; i++) {
                switch (i) {
                    case 0:
                        customers[i].Category=goodCategory;
                        break;
                    case 1:
                        customers[i].Category=veryGoodCategory;
                        break;
                    case 2:
                        customers[i].Category=bestCategory;
                        break;
                    case 3:
                        customers[i].Category=badCategory;
                        break;
                    case 4:
                        customers[i].Category=goodCategory;
                        break;
                    case 5:
                        customers[i].Category=veryGoodCategory;
                        break;
                    case 6:
                        customers[i].Category=bestCategory;
                        break;
                }
                customers[i].Save();
            }
        }
    }
}
