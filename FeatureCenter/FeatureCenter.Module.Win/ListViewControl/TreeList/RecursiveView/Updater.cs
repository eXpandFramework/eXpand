using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.RecursiveView {
    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            RVCategory category = CreateCategory("Parent", null);
            CreateCategory("Child1", category);
            CreateCategory("Child2", category);
        }
        private RVCategory CreateCategory(string name, HCategory parent)
        {
            var category = Session.FindObject<RVCategory>(new BinaryOperator("Name", name));
            if (category == null)
            {
                category = new RVCategory(Session) {Name = name, Parent = parent};
                category.Save();
                CreateCategorizedItem("Item1", category);
                CreateCategorizedItem("Item2", category);
            }
            return category;
        }
        private void CreateCategorizedItem(string name, RVCategory category)
        {
            string realName = name + " - " + category.Name;
            var item = Session.FindObject<RVItem>(new BinaryOperator("Name", realName));
            if (item == null)
            {
                item = new RVItem(Session) { Name = realName, Category = category };
                item.Save();
            }
        }
    }
}