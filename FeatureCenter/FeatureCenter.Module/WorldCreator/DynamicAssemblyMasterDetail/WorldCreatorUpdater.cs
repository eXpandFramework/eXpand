using DevExpress.Xpo;

namespace FeatureCenter.Module.WorldCreator.DynamicAssemblyMasterDetail
{
    public class WorldCreatorUpdater:Xpand.ExpressApp.WorldCreator.WorldCreatorUpdater
    {
        public const string MasterDetailDynamicAssembly = "MasterDetailDynamicAssembly";
        private const string DMDOrder = "DMDOrder";

        private const string DMDOrderLine = "DMDOrderLine";

        public const string DMDCustomer = "DMDCustomer";

        public WorldCreatorUpdater(Session session) : base(session) {
        }

        public override void Update() {
            new DynamicAssemblyBuilder(Session).Build(DMDCustomer, DMDOrder, DMDOrderLine, MasterDetailDynamicAssembly);
        }

    }

}
