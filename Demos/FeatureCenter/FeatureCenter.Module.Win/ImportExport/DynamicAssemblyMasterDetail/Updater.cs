using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.ImportExport;
using Xpand.Xpo;

namespace FeatureCenter.Module.Win.ImportExport.DynamicAssemblyMasterDetail {
    public class Updater : FCUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var session = ((XPObjectSpace)ObjectSpace).Session;
            if (session.FindObject<SerializationConfigurationGroup>(group => group.Name == "Dynamic Assembly Master Detail") == null) {
                var importEngine = new ImportEngine();
                importEngine.ImportObjects(info => ObjectSpace, GetType(), "DynamicAssemblyMasterDetailGroup.xml");
                importEngine.ImportObjects(info => ObjectSpace, GetType(), "DynamicAssemblyMasterDetailModel.xml");
                importEngine.ImportObjects(info => ObjectSpace, GetType(), "DynamicAssemblyMasterDetailModelGroup.xml");
            }
        }
    }
}