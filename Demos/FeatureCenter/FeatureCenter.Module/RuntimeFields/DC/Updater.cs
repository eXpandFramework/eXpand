using System;
using DevExpress.ExpressApp;

namespace FeatureCenter.Module.RuntimeFields.DC {
    public class Updater : FCUpdater {
        public Updater(IObjectSpace objectSpace, Version version, Xpand.Persistent.BaseImpl.Updater updater)
            : base(objectSpace, version, updater) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            if (ObjectSpace.FindObject<DomainComponentWithRuntimeMembers>(null) == null) {
                var members = ObjectSpace.CreateObject<DomainComponentWithRuntimeMembers>();
                members.Name = "Apostolis Bekiaris";
                members.Email = "apostolis.bekiaris@gmail.com";
                members.Twitter = "@tolisss";
                ObjectSpace.CommitChanges();
            }
        }
    }
}