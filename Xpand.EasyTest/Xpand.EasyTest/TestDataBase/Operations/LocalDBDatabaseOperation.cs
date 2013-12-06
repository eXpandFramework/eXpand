using System;
using System.Data.SqlClient;
using System.Threading;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.LocalDB {
    public class LocalDBDatabaseOperation : IEasyTestDatabaseOperation {
        const string DefaultInstance = "(localdb)\v11.0";
        const string DefaultConnString = "Server={0};integrated security=SSPI;initial catalog={1};";
        const string MasterDB = "master";

        protected virtual void KillConnections(TestDatabase database)
        {
            string serverName = database.Server;
            if (string.IsNullOrEmpty(serverName))
            {
                serverName = DefaultInstance;
            }
            using (SqlConnection connection = new SqlConnection(string.Format(DefaultConnString, serverName, MasterDB)))
            {
                connection.Open();
                try
                {
                    SqlCommand command = new SqlCommand("sp_who", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        int ordinal = reader.GetOrdinal("dbname");
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(ordinal) && (reader.GetString(ordinal) == database.DBName))
                            {
                                SqlConnection connection2 = new SqlConnection(connection.ConnectionString);
                                try
                                {
                                    try
                                    {
                                        connection2.Open();
                                        new SqlCommand("kill " + reader["spid"], connection2).ExecuteNonQuery();
                                    }
                                    catch
                                    {
                                    }
                                    continue;
                                }
                                finally
                                {
                                    connection2.Close();
                                }
                            }
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        
        public void Drop(TestDatabase database) {
            KillConnections(database);
            string serverName = database.Server;
            if (string.IsNullOrEmpty(serverName)) {
                serverName = DefaultInstance;
            }
            using (var connection = new SqlConnection(string.Format(DefaultConnString, serverName, MasterDB))) {
                connection.Open();
                connection.Open();
                using (SqlCommand command = new SqlCommand(string.Format("IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'{0}') DROP DATABASE [{0}]", database.DBName), connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public void Restore(TestDatabase database) {
            string serverName = database.Server;
            if (string.IsNullOrEmpty(serverName)) {
                serverName = DefaultInstance;
            }

            Drop(database);
            using (var connection = new SqlConnection(string.Format(DefaultConnString, serverName, MasterDB))) {
                connection.Open();
                if (string.IsNullOrEmpty(database.Backupfilename)) {
                    using (
                        var command = new SqlCommand(string.Format(@"CREATE DATABASE [{0}]", database.DBName),
                                                     connection)) {
                        command.ExecuteNonQuery();
                    }
                } else {
                    using (
                        var command =
                            new SqlCommand(
                                string.Format(
                                    @"RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH REPLACE, MOVE N'{0}' TO N'{2}\{0}.mdf', MOVE N'{0}_log' TO N'{2}\{0}_log.LDF'",
                                    database.DBName, database.Backupfilename,
                                    Environment.GetEnvironmentVariable("USERPROFILE")), connection)) {
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
                bool succes = false;
                int counter = 0;
                connection.ConnectionString = string.Format(DefaultConnString, serverName, database.DBName);
                do {
                    try {
                        connection.Open();
                        succes = true;
                    } catch (Exception) {
                    }
                    if (succes) {
                        connection.Close();
                    } else {
                        Thread.Sleep(200);
                    }
                    counter++;
                } while (!succes && counter < 50);
                if (!succes) {
                    connection.Open();
                }
            }
        }
    }
}
