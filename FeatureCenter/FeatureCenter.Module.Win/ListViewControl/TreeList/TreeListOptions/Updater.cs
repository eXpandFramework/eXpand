using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.TreeListOptions {
    public class Updater : ModuleUpdater {
        public Updater(Session session, Version currentDBVersion)
            : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (Session.FindObject<OCategory>(null) == null) {
                var category = new OCategory(Session) { Name = "1", FullName = "Group 1" };
                category.Save();
                new OCategory(Session) { Name = "1.1", Parent = category, FullName = "Text 1.1", MoreInfo = "moreinfo 1.1", MoreInfo2 = "moreinfo2 1.1" }.Save();
                new OCategory(Session) { Name = "1.2", Parent = category, FullName = "Text 1.2", MoreInfo = "moreinfo 1.2", MoreInfo2 = "moreinfo 1.2" }.Save();
                category = new OCategory(Session) { Name = "2", FullName = "Group 2" };
                category.Save();
                new OCategory(Session) { Name = "2.1", Parent = category }.Save();
                new OCategory(Session) { Name = "2.2", Parent = category }.Save();
            }
        }
    }
}
