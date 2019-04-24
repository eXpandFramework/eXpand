using System.Data.SqlClient;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class SqlDropDatabaseCommand : SqlCommand {
        public new const string Name = "SqlDropDB";

        protected override void InternalExecute(ICommandAdapter adapter){
            var dbName = this.ParameterValue<string>("DBName");
            Dropdatabase(dbName);
        }

        public static void Dropdatabase(string dbName){
            using (var sqlConnection = new SqlConnection(ConnectionString)){
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand()){
                    sqlCommand.CommandText = "ALTER DATABASE " + dbName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    try{
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (SqlException){
                        return;
                    }
                    sqlCommand.CommandText = "Drop database " + dbName;
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}