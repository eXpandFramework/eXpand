using DevExpress.EasyTest.Framework;
using Xpand.EasyTest.TestDataBase.Operations;

namespace Xpand.ExpressApp.EasyTest.WebAdapter {
    public class TestASADatabase : TestDatabase {
        protected override string GetDataBaseUtilType() {
            return typeof (ASADatabaseOperations).FullName;
        }
    }

    public class ASADatabaseOperations : ASADatabaseOperation {
    }

    public class TestLocalDBDatabase : TestDatabase {
        protected override string GetDataBaseUtilType() {
            return typeof (LocalDBDatabaseOperations).FullName;
        }
    }

    public class LocalDBDatabaseOperations : LocalDBDatabaseOperation {
    }

    public class TestSQLiteDatabase : TestDatabase {
        protected override string GetDataBaseUtilType() {
            return typeof (SQLiteDatabaseOperations).FullName;
        }
    }

    public class SQLiteDatabaseOperations : SQLiteDatabaseOperation {
    }
}