using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.RecursiveView {
    public class Updater : FCUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            RVCategory category = CreateCategory("Parent", null);
            CreateCategory("Child1", category);
            CreateCategory("Child2", category);
            ObjectSpace.CommitChanges();
        }
        private RVCategory CreateCategory(string name, HCategory parent) {
            var session = ((XPObjectSpace)ObjectSpace).Session;
            var category = session.FindObject<RVCategory>(new BinaryOperator("Name", name));
            if (category == null) {
                category = new RVCategory(session) { Name = name, Parent = parent };
                CreateCategorizedItem("Item1", category);
                CreateCategorizedItem("Item2", category);
            }
            return category;
        }
        private void CreateCategorizedItem(string name, RVCategory category) {
            string realName = name + " - " + category.Name;
            var session = ((XPObjectSpace)ObjectSpace).Session;
            var item = session.FindObject<RVItem>(new BinaryOperator("Name", realName));
            if (item == null) {
                new RVItem(session) { Name = realName, Category = category };
            }
        }
    }
}