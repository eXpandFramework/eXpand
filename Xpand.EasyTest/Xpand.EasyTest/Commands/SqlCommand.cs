using System;
using System.Data.SqlClient;
using System.Linq;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands {
    public class SqlCommand : Command {
        private string _scriptsPath;
        public const string Name = "Sql";

        protected string GetConnectionString() {
            const string connectionString = @"Integrated Security=SSPI;Data Source=(localDb)\MSSQLLocalDB;";
            if (Parameters.MainParameter != null && !string.IsNullOrEmpty(Parameters.MainParameter.Value)) {
                return Parameters.MainParameter.Value.StartsWith("Alias:")
                    ? Extensions.GetAlias(_scriptsPath,
                        Parameters.MainParameter.Value.Substring(
                            Parameters.MainParameter.Value.IndexOf(":", StringComparison.Ordinal) + 1)).Value
                    : Parameters.MainParameter.Value;
            }
            return connectionString;
        }

        public override void ParseCommand(CommandCreationParam commandCreationParam) {
            base.ParseCommand(commandCreationParam);
            _scriptsPath = commandCreationParam.TestParameters.ScriptsPath;
        }

        protected override void InternalExecute(ICommandAdapter adapter) {

            using (var sqlConnection = new SqlConnection(GetConnectionString())) {
                sqlConnection.Open();
                foreach (var parameter in Parameters.Where(parameter => parameter.Name.Contains("Command"))) {
                    using (var sqlCommand = sqlConnection.CreateCommand()) {
                        sqlCommand.CommandText = parameter.Value;
                        if (parameter.Name.StartsWith("Scalar")) {
                            var scalar = sqlCommand.ExecuteScalar();
                            if (scalar + "" != Parameters["Result" + parameter.Name.Replace("ScalarCommand", "")].Value)
                                throw new CommandException(string.Format("{0} result is {1}", parameter.Name, scalar), StartPosition);
                        }
                        else
                            sqlCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}