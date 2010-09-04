using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

using Xpand.Xpo;

namespace FeatureCenter.Module.WorldCreator.NorthWind {
    public class WorldCreatorUpdater : Xpand.ExpressApp.WorldCreator.WorldCreatorUpdater
    {
        private const string NorthWind = "NorthWind";
        public WorldCreatorUpdater(Session session) : base(session) {
        }
        public override void Update()
        {
            if (Session.FindObject<PersistentAssemblyInfo>(info => info.Name == NorthWind) != null) return;
            new ImportEngine().ImportObjects(GetType().Assembly.GetManifestResourceStream(GetType(),NorthWind+".xml"),new UnitOfWork(Session.DataLayer));
        }
    }
}