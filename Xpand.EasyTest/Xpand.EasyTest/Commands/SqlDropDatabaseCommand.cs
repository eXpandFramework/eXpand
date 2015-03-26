using System.Data.SqlClient;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class SqlDropDatabaseCommand : SqlCommand {
        public new const string Name = "SqlDropDB";

        protected override void InternalExecute(ICommandAdapter adapter) {
            using (var sqlConnection = new SqlConnection(GetConnectionString())) {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand()) {
                    var dbName = this.ParameterValue<string>("DBName");
                    sqlCommand.CommandText = "ALTER DATABASE " + dbName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    try {
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (SqlException) {
                        return;
                    }
                    sqlCommand.CommandText = "Drop database " + dbName;
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}