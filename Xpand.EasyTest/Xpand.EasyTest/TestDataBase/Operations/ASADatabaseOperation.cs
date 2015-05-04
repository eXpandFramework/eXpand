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
                        if (string.IsNullOrEmpty(database.Backupfilename)) {
                            c.CommandText = string.Format("Create Database '{0}'", dbFileName);
                            c.ExecuteNonQuery();
                        } else {
                            if (
                                GetExtension(database.Backupfilename)
                                    .Equals(".db", StringComparison.OrdinalIgnoreCase) ||
                                File.Exists(database.Backupfilename + ".db")) {
                                string baseSourceFileName = database.Backupfilename;
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
                                string backupFileName = database.Backupfilename.Replace(@"\", @"\\");
                                if (GetExtension(database.Backupfilename)
                                        .Equals(".1", StringComparison.OrdinalIgnoreCase)) {
                                    backupFileName = Path.ChangeExtension(backupFileName, null);
                                }
                                c.CommandText = string.Format("RESTORE DATABASE '{0}' FROM '{1}' HISTORY OFF",
                                                              dbFileName, backupFileName);
                                c.ExecuteNonQuery();
                            }
                        }
                        c.CommandText = string.Format("START DATABASE '{0}' AS [{1}]", dbFileName, database.DBName);
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
                string uid;
                string pwd;
                if (database.Login != null && !string.IsNullOrEmpty(database.Login.UserID)) {
                    uid = database.Login.UserID;
                } else {
                    uid = "dba";
                }
                if (database.Login != null && !string.IsNullOrEmpty(database.Login.Password)) {
                    pwd = database.Login.Password;
                } else {
                    pwd = "sql";
                }
                connString = string.Format("eng={0};uid={1};pwd={2};", database.Server, uid, pwd);
            }
            return "DBN=utility_db;" + connString;
        }

        void DropDB(IDbConnection conn, TestDatabase database) {
            using (IDbCommand c = conn.CreateCommand()) {
                c.CommandText = string.Format("STOP DATABASE [{0}] UNCONDITIONALLY", database.DBName);
                try {
                    c.ExecuteNonQuery();
                } catch (Exception) {
                }
                if (!string.IsNullOrEmpty(database.DBSourceLocation)) {
                    c.CommandText = string.Format("DROP DATABASE '{0}'", database.DBSourceLocation.Replace(@"\", @"\\"));
                    try {
                        c.ExecuteNonQuery();
                    } catch (Exception) {
                    }
                }
            }
        }
    }
}