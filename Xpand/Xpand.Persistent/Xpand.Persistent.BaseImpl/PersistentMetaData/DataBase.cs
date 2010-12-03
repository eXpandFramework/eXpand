using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [NonPersistent]
    public class DataBase : XpandBaseCustomObject, IDataBase {
        public DataBase(Session session)
            : base(session) {
        }
        private string _name;

        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
    }
}