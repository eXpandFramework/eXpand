using System;
using System.Data;
using System.IO;
using DevExpress.EasyTest.Framework;
using DevExpress.Xpo.DB.Helpers;

namespace Xpand.EasyTest.TestDataBase.Operations {
    public class ASADatabaseOperation : IEasyTestDatabaseOperation {
        private const string AssemblyName = "iAnywhere.Data.SQLAnywhere";
        private const string TypeName = "iAnywhere.Data.SQLAnywhere.SAConnection";
        public void Drop(TestDatabase database) {
            var connectionString = ParseConnectionString(database);
            using (var conn = database.DbConnection(connectionString, AssemblyName, TypeName)) {
                conn.Open();
                try {
                    DropDB(conn, database);
                } finally {
                    conn.Close();
                }
            }
        }

        public void Restore(TestDatabase database) {
            var connectionString = ParseConnectionString(database);
            using (var conn = database.DbConnection(connectionString, AssemblyName,TypeName)) {
                conn.Open();
                try {
                    DropDB(conn, database);
                    using (IDbCommand c = conn.CreateCommand()) {
                        string dbFileName = database.DBSourceLocation.Replace(@"\", @"\\");
                        if (string.IsNullOrEmpty(database.BackupFileName)) {
                            c.CommandText = $"Create Database '{dbFileName}'";
                            c.ExecuteNonQuery();
                        } else {
                            if (
                                GetExtension(database.BackupFileName)
                                    .Equals(".db", StringComparison.OrdinalIgnoreCase) ||
                                File.Exists(database.BackupFileName + ".db")) {
                                string baseSourceFileName = database.BackupFileName;
                                string baseDestFileName = database.DBSourceLocation;
                                if (GetExtension(baseSourceFileName)
                                        .Equals(".db", StringComparison.OrdinalIgnoreCase)) {
                                    baseSourceFileName = Path.ChangeExtension(baseSourceFileName, null);
                                }
                                if (GetExtension(baseDestFileName)
                                        .Equals(".db", StringComparison.OrdinalIgnoreCase)) {
                                    baseDestFileName = Path.ChangeExtension(baseDestFileName, null);
                                }
                                File.Copy(baseSourceFileName + ".db", baseDestFileName + ".db", true);
                                if (File.Exists(baseSourceFileName + ".log")) {
                                    File.Copy(baseSourceFileName + ".log", baseDestFileName + ".log", true);
                                }
                            } else {
                                string backupFileName = database.BackupFileName.Replace(@"\", @"\\");
                                if (GetExtension(database.BackupFileName)
                                        .Equals(".1", StringComparison.OrdinalIgnoreCase)) {
                                    backupFileName = Path.ChangeExtension(backupFileName, null);
                                }
                                c.CommandText = $"RESTORE DATABASE '{dbFileName}' FROM '{backupFileName}' HISTORY OFF";
                                c.ExecuteNonQuery();
                            }
                        }
                        c.CommandText = $"START DATABASE '{dbFileName}' AS [{database.DBName}]";
                        c.ExecuteNonQuery();
                    }
                } finally {
                    conn.Close();
                }
            }
        }

        

        string GetExtension(string path) {
            return Path.GetExtension(path);
        }

        string ParseConnectionString(TestDatabase database) {
            string connString;
            if (!string.IsNullOrEmpty(database.ConnectionString)) {
                var helper = new ConnectionStringParser(database.ConnectionString);
                helper.RemovePartByName("DatabaseName");
                helper.RemovePartByName("DBN");
                helper.RemovePartByName("DatabaseFile");
                helper.RemovePartByName("DBF");
                connString = helper.GetConnectionString();
            } else {
                var uid = !string.IsNullOrEmpty(database.Login?.UserID) ? database.Login.UserID : "dba";
                var pwd = !string.IsNullOrEmpty(database.Login?.Password) ? database.Login.Password : "sql";
                connString = $"eng={database.Server};uid={uid};pwd={pwd};";
            }
            return "DBN=utility_db;" + connString;
        }

        void DropDB(IDbConnection conn, TestDatabase database) {
            using (IDbCommand c = conn.CreateCommand()) {
                c.CommandText = $"STOP DATABASE [{database.DBName}] UNCONDITIONALLY";
                try {
                    c.ExecuteNonQuery();
                } catch (Exception) {
                }
                if (!string.IsNullOrEmpty(database.DBSourceLocation)) {
                    c.CommandText = $"DROP DATABASE '{database.DBSourceLocation.Replace(@"\", @"\\")}'";
                    try {
                        c.ExecuteNonQuery();
                    } catch (Exception) {
                    }
                }
            }
        }
    }
}