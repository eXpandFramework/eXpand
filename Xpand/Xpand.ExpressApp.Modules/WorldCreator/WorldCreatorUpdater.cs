using DevExpress.Xpo;

namespace Xpand.ExpressApp.WorldCreator {
    public abstract class WorldCreatorUpdater {
        readonly Session _session;

        protected WorldCreatorUpdater(Session session) {
            _session = session;
        }

        public Session Session {
            get { return _session; }
        }

        public abstract void Update();

        
    }
}