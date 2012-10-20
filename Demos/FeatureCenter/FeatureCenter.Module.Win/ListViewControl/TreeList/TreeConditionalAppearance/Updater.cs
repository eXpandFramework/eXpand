using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.TreeConditionalAppearance {
    public class Updater : ModuleUpdater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.Session.FindObject<TCACategory>(null) == null) {
                var category = new TCACategory(ObjectSpace.Session) { Name = "1", FullName = "Group 1" };
                new TCACategory(ObjectSpace.Session) { Name = "1.1", Parent = category, FullName = "Text 1.1", MoreInfo = "moreinfo 1.1", MoreInfo2 = "moreinfo2 1.1" };
                new TCACategory(ObjectSpace.Session) { Name = "1.2", Parent = category, FullName = "Text 1.2", MoreInfo = "moreinfo 1.2", MoreInfo2 = "moreinfo 1.2" };
                category = new TCACategory(ObjectSpace.Session) { Name = "2", FullName = "Group 2" };
                new TCACategory(ObjectSpace.Session) { Name = "2.1", Parent = category };
                new TCACategory(ObjectSpace.Session) { Name = "2.2", Parent = category };
                ObjectSpace.CommitChanges();
            }
        }
    }
}
