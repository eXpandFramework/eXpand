using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace FeatureCenter.Module.Win.ImportExport.DynamicAssemblyMasterDetail {
    public class WorldCreatorUpdater : WorldCreatorModuleUpdater {
        public const string MasterDetailDynamicAssembly = "IOMasterDetailDynamicAssembly";

        public const string DMDCustomer = "IODMDCustomer";
        public const string DMDOrder = "IODMDOrder";

        public const string DMDOrderLine = "IODMDOrderLine";

        public WorldCreatorUpdater(IObjectSpace objectSpace, Version version)
            : base(objectSpace,version) {
        }


        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();

            if (ObjectSpace.QueryObject<PersistentAssemblyInfo>(info => info.Name == MasterDetailDynamicAssembly) == null) {
                var importEngine = new ImportEngine();
                importEngine.ImportObjects(info => ObjectSpace, GetType(), "DynamicAssemblyMasterDetail.xml");
            }

        }
    }
}
