using System.Data.SqlClient;
using System.Linq;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class SqlCommand:Command{
        public const string Name = "Sql";
        protected override void InternalExecute(ICommandAdapter adapter){
            var sqlConnection = new SqlConnection(Parameters.MainParameter.Value);
            sqlConnection.Open();
            foreach (var parameter in Parameters.Where(parameter => parameter.Name.Contains("Command"))){
                var sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = parameter.Value;
                if (parameter.Name.StartsWith("Scalar")){
                    var scalar = sqlCommand.ExecuteScalar();
                    if (scalar+""!=Parameters["Result"+parameter.Name.Replace("ScalarCommand","")].Value)
                        throw new CommandException(string.Format("{0} result is {1}", parameter.Name, scalar),StartPosition);
                }
                else
                    sqlCommand.ExecuteNonQuery();
            }
        }
    }
}