using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.TreeListOptions {
    public class Updater : FCUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion, Xpand.Persistent.BaseImpl.Updater updater)
            : base(objectSpace, currentDBVersion,updater) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var session = ((XPObjectSpace)ObjectSpace).Session;
            if (session.FindObject<OCategory>(null) == null) {
                var category = new OCategory(session) { Name = "1", FullName = "Group 1" };
                new OCategory(session) { Name = "1.1", Parent = category, FullName = "Text 1.1", MoreInfo = "moreinfo 1.1", MoreInfo2 = "moreinfo2 1.1" };
                new OCategory(session) { Name = "1.2", Parent = category, FullName = "Text 1.2", MoreInfo = "moreinfo 1.2", MoreInfo2 = "moreinfo 1.2" };
                category = new OCategory(session) { Name = "2", FullName = "Group 2" };
                new OCategory(session) { Name = "2.1", Parent = category };
                new OCategory(session) { Name = "2.2", Parent = category };
                ObjectSpace.CommitChanges();
            }
        }
    }
}
