using System;
using System.IO;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.TestDataBase.Operations {
    public class SQLiteDatabaseOperation : IEasyTestDatabaseOperation {
        public void Drop(TestDatabase database) {
            try {
                File.Delete(database.DBSourceLocation);
            } catch (Exception) {
            }
        }

        public void Restore(TestDatabase database) {
            File.Copy(database.Backupfilename, database.DBSourceLocation, true);
        }
    }
}