using DevExpress.EasyTest.Framework;
using Xpand.EasyTest.TestDataBase.Operations;

namespace Xpand.EasyTest.TestDataBase {
    public class TestSQLiteDatabase : TestDatabase {
        public override string GetDataBaseUtilType() {
            return typeof(SQLiteDatabaseOperation).FullName;
        }
    }

    public class TestMySQLDatabase:TestDatabase{
        public override string GetDataBaseUtilType(){
            return typeof(MySQLDatabaseOperation).FullName;
        }
    }

    public class TestLocalDBDatabase : TestDatabase {
        public override string GetDataBaseUtilType() {
            return typeof(LocalDBDatabaseOperation).FullName;
        }
    }

    public class TestASADatabase : TestDatabase {
        public override string GetDataBaseUtilType() {
            return typeof(ASADatabaseOperation).FullName;
        }
    }

}
