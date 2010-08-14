using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Xpo;

namespace FeatureCenter.Module.ImportExport.DynamicAssemblyMasterDetail
{
    public class WorldCreatorUpdater:eXpand.ExpressApp.WorldCreator.WorldCreatorUpdater
    {
        public const string MasterDetailDynamicAssembly = "IOMasterDetailDynamicAssembly";

        public const string DMDCustomer = "IODMDCustomer";
        public const string DMDOrder = "IODMDOrder";

        public const string DMDOrderLine = "IODMDOrderLine";

        public WorldCreatorUpdater(Session session) : base(session) {
        }


        public override void Update() {
            if (Session.FindObject<PersistentAssemblyInfo>(info => info.Name==MasterDetailDynamicAssembly) == null){
                var importEngine = new ImportEngine();
                var unitOfWork = new UnitOfWork(Session.DataLayer);
                importEngine.ImportObjects(unitOfWork, GetType(), "DynamicAssemblyMasterDetail.xml");
            }
        }







    }
}
