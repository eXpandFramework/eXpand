using System;
using System.Data.SqlClient;
using System.Linq;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands {
    public class SqlCommand : Command {
        public const string ConnectionString = @"Integrated Security=SSPI;Data Source=(localDb)\MSSQLLocalDB;";
        private string _scriptsPath;
        public const string Name = "Sql";

        protected string GetConnectionString(string mainParameterValue) {
            
            if (!string.IsNullOrEmpty(mainParameterValue)) {
                return mainParameterValue.StartsWith("Alias:")
                    ? Extensions.GetAlias(_scriptsPath,
                        mainParameterValue.Substring(
                            mainParameterValue.IndexOf(":", StringComparison.Ordinal) + 1)).Value
                    : mainParameterValue;
            }
            return ConnectionString;
        }

        public override void ParseCommand(CommandCreationParam commandCreationParam) {
            base.ParseCommand(commandCreationParam);
            _scriptsPath = commandCreationParam.TestParameters.ScriptsPath;
        }

        protected override void InternalExecute(ICommandAdapter adapter) {

            using (var sqlConnection = new SqlConnection(GetConnectionString(Parameters.MainParameter?.Value))) {
                sqlConnection.Open();
                foreach (var parameter in Parameters.Where(parameter => parameter.Name.Contains("Command"))) {
                    using (var sqlCommand = sqlConnection.CreateCommand()) {
                        sqlCommand.CommandText = parameter.Value;
                        if (parameter.Name.StartsWith("Scalar")) {
                            var scalar = sqlCommand.ExecuteScalar();
                            if (scalar + "" != Parameters["Result" + parameter.Name.Replace("ScalarCommand", "")].Value)
                                throw new CommandException($"{parameter.Name} result is {scalar}", StartPosition);
                        }
                        else
                            sqlCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}