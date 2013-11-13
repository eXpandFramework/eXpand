using DevExpress.EasyTest.Framework;
using Xpand.EasyTest.ASA;
using Xpand.EasyTest.LocalDB;
using Xpand.EasyTest.SQLite;

namespace Xpand.EasyTest.TestDataBase {
    public class TestSQLiteDatabase : TestDatabase {
        protected override string GetDataBaseUtilType() {
            return typeof(SQLiteDatabaseOperation).FullName;
        }
    }
    public class TestLocalDBDatabase : TestDatabase {
        protected override string GetDataBaseUtilType() {
            return typeof(LocalDBDatabaseOperation).FullName;
        }
    }
    public class TestASADatabase : TestDatabase {
        protected override string GetDataBaseUtilType() {
            return typeof(ASADatabaseOperation).FullName;
        }
    }

}
