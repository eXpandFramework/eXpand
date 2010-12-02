using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.RecursiveView {
    public class Updater : ModuleUpdater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion)
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
            var category = ObjectSpace.Session.FindObject<RVCategory>(new BinaryOperator("Name", name));
            if (category == null) {
                category = new RVCategory(ObjectSpace.Session) { Name = name, Parent = parent };
                CreateCategorizedItem("Item1", category);
                CreateCategorizedItem("Item2", category);
            }
            return category;
        }
        private void CreateCategorizedItem(string name, RVCategory category) {
            string realName = name + " - " + category.Name;
            var item = ObjectSpace.Session.FindObject<RVItem>(new BinaryOperator("Name", realName));
            if (item == null) {
                new RVItem(ObjectSpace.Session) { Name = realName, Category = category };
            }
        }
    }
}