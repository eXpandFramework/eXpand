using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Xpo;

namespace FeatureCenter.Module.Win.ImportExport.DynamicAssemblyMasterDetail {
    public class WorldCreatorUpdater : Xpand.ExpressApp.WorldCreator.WorldCreatorUpdater {
        public const string MasterDetailDynamicAssembly = "IOMasterDetailDynamicAssembly";

        public const string DMDCustomer = "IODMDCustomer";
        public const string DMDOrder = "IODMDOrder";

        public const string DMDOrderLine = "IODMDOrderLine";

        public WorldCreatorUpdater(Session session)
            : base(session) {
        }


        public override void Update() {
            if (Session.FindObject<PersistentAssemblyInfo>(info => info.Name == MasterDetailDynamicAssembly) == null) {
                var importEngine = new ImportEngine();
                importEngine.ImportObjects(new ObjectSpace(XafTypesInfo.Instance, XafTypesInfo.XpoTypeInfoSource, () => new UnitOfWork(Session.DataLayer)), GetType(), "DynamicAssemblyMasterDetail.xml");
            }
        }







    }
}
