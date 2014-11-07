using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General;

namespace JobsSchedulerTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<SecuritySystemRole>(null) == null) {
                CreateSecurityObjects();
                var connectionString = ApplicationHelper.Instance.Application.ConnectionString;
                using (var sqlConnection = new SqlConnection(connectionString)) {
                    sqlConnection.Open();
                    using (var command = sqlConnection.CreateCommand()) {
                        CreateQuartzDatabase(command);
                    }
                }
            }
        }

        private void CreateQuartzDatabase(IDbCommand command) {
            command.CommandText = "IF EXISTS(select * from sys.databases where name='Quartz') ALTER DATABASE [Quartz] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
            command.ExecuteScalar();
            command.CommandText = "IF EXISTS(select * from sys.databases where name='Quartz') DROP Database [Quartz]";
            command.ExecuteScalar();
            command.CommandText = "CREATE DATABASE [Quartz]";
            command.ExecuteScalar();
            var script = File.ReadAllText(Path.Combine(this.XpandRootPath(), @"Support\quartz\tables_sqlServer.sql"));
            var commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (string commandString in commandStrings) {
                if (commandString.Trim() != "") {
                    command.CommandText = commandString;
                    command.ExecuteNonQuery();
                }
            }
        }

        private void CreateSecurityObjects() {
            var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();

            var adminRole = ObjectSpace.GetAdminRole("Admin");
            adminRole.GetUser("Admin");

            var userRole = ObjectSpace.GetRole("User");
            var user = (SecuritySystemUser)userRole.GetUser("user");
            user.Roles.Add(defaultRole);
        }
    }
}
