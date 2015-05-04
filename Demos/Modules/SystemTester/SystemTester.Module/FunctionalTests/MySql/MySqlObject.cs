using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.FunctionalTests.MySql {
    [DefaultClassOptions]
    public class MySQLObject : BaseObject{
        public MySQLObject(Session session) : base(session){
        }

        public bool IsMySQL{
            get { return ((MultiDataStoreProxy) ((BaseDataLayer)Session.DataLayer).ConnectionProvider).DataStore is MySqlConnectionProvider; }
        }
    }
}
