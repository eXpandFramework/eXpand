using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.SqlDBMapper;

namespace eXpand.Persistent.BaseImpl.SqlDBMapper {
    [NonPersistent]
    public class DataBase : BaseObject, IDataBase {
        public DataBase(Session session) : base(session) {
        }
        private string _name;

        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
    }
}