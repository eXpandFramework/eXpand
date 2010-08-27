using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Xpo;

namespace FeatureCenter.Module.WorldCreator.NorthWind {
    public class WorldCreatorUpdater : eXpand.ExpressApp.WorldCreator.WorldCreatorUpdater
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