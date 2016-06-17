using System;
using DevExpress.ExpressApp;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.System.DatabaseUpdate {
    public class ModuleUpdater:WorldCreatorModuleUpdater {
        public ModuleUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion){
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (CurrentDBVersion > new Version(15, 2, 9) && CurrentDBVersion < new Version(15, 2, 10)){
                var objects = ObjectSpace.QueryObjects<IPersistentAssemblyInfo>(info => info.Revision==0);
                foreach (var assemblyInfo in objects){
                    assemblyInfo.Revision++;
                }
                ObjectSpace.CommitChanges();
            }
        }
    }
}
