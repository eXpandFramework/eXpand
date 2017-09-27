using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.ModelDifference;

namespace Xpand.ExpressApp.ModelDifference.DatabaseUpdate {
    public class DeviceCategoryUpdater:ModuleUpdater {
        public DeviceCategoryUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion){
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (CurrentDBVersion <= new Version("17.1.6.4")){
                var modelDifferenceObjects = ObjectSpace.GetObjectsQuery<ModelDifferenceObject>().ToArray().Where(o => o.DeviceCategory==DeviceCategory.All).ToArray();
                foreach (var modelDifferenceObject in modelDifferenceObjects){
                    modelDifferenceObject.DeviceCategory=DeviceCategory.Desktop;
                }
                ObjectSpace.CommitChanges();
                foreach (var modelDifferenceObject in modelDifferenceObjects){
                    modelDifferenceObject.DeviceCategory=DeviceCategory.All;
                }
                ObjectSpace.CommitChanges();
            }
        }
    }
}
