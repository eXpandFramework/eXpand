using DevExpress.Xpo;

namespace eXpand.ExpressApp.WorldCreator {
    public abstract class WorldCreatorUpdater {
        readonly Session _session;

        protected WorldCreatorUpdater(Session session) {
            _session = session;
        }

        public Session Session {
            get { return _session; }
        }

        public virtual void CreatePersistentAssemblies() {
        }

        public virtual void CreateDynamicMembers() {
        }

        public virtual void UpdateDynamicMembers() {
        }
    }
}