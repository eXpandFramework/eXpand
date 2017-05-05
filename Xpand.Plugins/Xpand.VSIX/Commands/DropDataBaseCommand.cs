using System;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.DXCore.Controls.Xpo.DB.Helpers;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Xpand.VSIX.VSPackage;
using Task = System.Threading.Tasks.Task;

namespace Xpand.VSIX.Commands {
    public class DropDataBaseCommand:OleMenuCommand {
        private static readonly DTE2 _dte=DteExtensions.DTE;

        public DropDataBaseCommand():base((sender, args) => Drop(), new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidDropDatabase)){
            this.EnableForConfigFile();
        }

        static void Drop() {
            _dte.InitOutputCalls("Dropdatabase");
            Task.Factory.StartNew(() => {
                var startUpProject = _dte.Solution.FindStartUpProject();
                var configItem = startUpProject.ProjectItems.Cast<ProjectItem>()
                    .FirstOrDefault(item => new[] { "app.config", "web.config" }.Contains(item.Name.ToLower()));
                if (configItem == null) {
                    _dte.WriteToOutput("Startup project " + startUpProject.Name + " does not contain a config file");
                    return;
                }
                foreach (ConnectionString optionsConnectionString in OptionClass.Instance.ConnectionStrings) {
                    if (!string.IsNullOrEmpty(optionsConnectionString.Name)) {
                        var connectionStringSettings = GetConnectionStringSettings(configItem, optionsConnectionString.Name);
                        if (connectionStringSettings != null) {
                            try {
                                if (DbExist(connectionStringSettings)) {
                                    DropSqlServerDatabase(connectionStringSettings);
                                }
                            }
                            catch (Exception e) {
                                _dte.WriteToOutput(connectionStringSettings.ConnectionString + Environment.NewLine + e);
                            }
                        }
                    }
                }
            },CancellationToken.None,TaskCreationOptions.None,TaskScheduler.Default);
        }

        private static ConnectionStringSettings GetConnectionStringSettings(ProjectItem item, string name) {
            Property property = item.FindProperty(ProjectItemProperty.FullPath);
            var exeConfigurationFileMap = new ExeConfigurationFileMap { ExeConfigFilename = property.Value.ToString() };
            var configuration = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
            ConnectionStringsSection strings = configuration.ConnectionStrings;
            return strings.ConnectionStrings[name];
        }

        private static void DropSqlServerDatabase(ConnectionStringSettings connectionStringSettings) {
            _dte.WriteToOutput("Attempting connection with " + connectionStringSettings.ConnectionString);
            var parser = new ConnectionStringParser(connectionStringSettings.ConnectionString.ToLower());
            parser.RemovePartByName("xpoprovider");
            var database = parser.GetPartByName("initial catalog");
            parser.RemovePartByName("initial catalog");
            using (var connection = new SqlConnection(parser.GetConnectionString())) {
                connection.Open();
                using (SqlCommand sqlCommand = connection.CreateCommand()) {
                    sqlCommand.CommandText = $"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.CommandText = $"DROP DATABASE [{database}]";
                    sqlCommand.ExecuteNonQuery();
                    _dte.WriteToOutput(database + " dropped successfully");
                }
            }
        }

        private static bool DbExist(ConnectionStringSettings connectionStringSettings) {
            var parser = new ConnectionStringParser(connectionStringSettings.ConnectionString.ToLower());
            var database = parser.GetPartByName("initial catalog");
            parser.RemovePartByName("initial catalog");
            parser.RemovePartByName("xpoprovider");
            _dte.WriteToOutput("ConnectionStrings name:" + connectionStringSettings.Name + " data source: " + parser.GetPartByName("data source"));
            using (var connection = new SqlConnection(parser.GetConnectionString())) {
                connection.Open();
                object result;
                using (var sqlCommand = connection.CreateCommand()) {
                    sqlCommand.CommandText = $"SELECT database_id FROM sys.databases WHERE Name = '{database}'";
                    result = sqlCommand.ExecuteScalar();
                }
                var exists = result != null && int.Parse(result + "") > 0;
                string doesNot = exists ? null : " does not";
                _dte.WriteToOutput("Database " + database + doesNot + " exists");
                return exists;
            }
        }
    }
}
