using System;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.TestDataBase.Operations{
    public class MySQLDatabaseOperation:IEasyTestDatabaseOperation {
        private const string AssemblyName = "MySql.Data";
        private const string TypeName = "MySql.Data.MySqlClient.MySqlConnection";
        public void Restore(TestDatabase database){
            throw new NotImplementedException();
        }

        protected virtual string GetConnectionString(TestDatabase database) {
            if ((database.Login != null) && !string.IsNullOrEmpty(database.Login.UserID)) {
                return string.Format("Server={0};Uid={1};password={2}",database.Server,database.Login.UserID,database.Login.Password );
            }
            return string.Format("Server={0};IntegratedSecurity=yes;Uid=auth_windows",database.Server );
        }

        public void Drop(TestDatabase database){

            using (var conn = database.DbConnection(GetConnectionString(database),AssemblyName, TypeName)) {
                conn.Open();
                try {
                    using (var dbCommand = conn.CreateCommand()) {
                        dbCommand.CommandText = "DROP DATABASE IF EXISTS " + database.DBName;
                        dbCommand.ExecuteNonQuery();
                    }
                }
                finally {
                    conn.Close();
                }
            }

        }

    }
}