using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.PlugInCore;
using DevExpress.DXCore.Controls.Xpo;
using DevExpress.DXCore.Controls.Xpo.DB;
using DevExpress.DXCore.Controls.Xpo.DB.Exceptions;
using EnvDTE;
using eXpandAddIns.Enums;
using eXpandAddIns.Extensioons;
using Process=System.Diagnostics.Process;
using Project=EnvDTE.Project;
using Property=EnvDTE.Property;
using System.Linq;
using Configuration=System.Configuration.Configuration;
using ConfigurationManager=System.Configuration.ConfigurationManager;
using ConfigurationProperty=eXpandAddIns.Enums.ConfigurationProperty;

namespace eXpandAddIns
{
    public partial class PlugIn1 : StandardPlugIn
    {
        #region InitializePlugIn

        #endregion
        #region FinalizePlugIn
        #endregion

        private void convertProject_Execute(ExecuteEventArgs ea)
        {
            using (var storage = new DecoupledStorage(typeof (Options)))
            {
                string path = storage.ReadString(Options.GetPageName(), "projectConverterPath");
                string token = storage.ReadString(Options.GetPageName(), "token");
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(token))
                {
                    var directoryName = Path.GetDirectoryName(CodeRush.Solution.Active.FileName);
                    
                    var userName = string.Format("/s /k:{0} \"{1}\"", token, directoryName);
                    Process.Start(path, userName);
                    actionHint1.Text = "Project Converter Started !!!";
                    Rectangle rectangle = Screen.PrimaryScreen.Bounds;
                    actionHint1.PointTo(new Point(rectangle.Width / 2, rectangle.Height / 2));
                }
            }   
        }

        private void collapseAllItemsInSolutionExplorer_Execute(ExecuteEventArgs ea)
        {
            CodeRush.ApplicationObject.Solution.CollapseAllFolders();
        }

        private void exploreXafErrors_Execute(ExecuteEventArgs ea)
        {
            Project startUpProject = CodeRush.ApplicationObject.Solution.FindStartUpProject();
            Property outPut = startUpProject.ConfigurationManager.ActiveConfiguration.FindProperty(ConfigurationProperty.OutputPath);
            Property fullPath = startUpProject.FindProperty(ProjectProperty.FullPath);
            string path = Path.Combine(fullPath.Value.ToString(),outPut.Value.ToString());
            var reader = new InverseReader(Path.Combine(path,"expressAppFrameWork.log"));
            var stackTrace = new List<string>();
            while (!reader.SOF) {
                string readline = reader.Readline();
                stackTrace.Add(readline);
                if (readline.Trim().StartsWith("The error occured:") || readline.Trim().StartsWith("The error occurred:"))
                {
                    stackTrace.Reverse();
                    string errorMessage = "";
                    foreach (string trace in stackTrace) {
                        errorMessage += trace + Environment.NewLine;
                        if (trace.Trim().StartsWith("----------------------------------------------------"))
                            break;
                    }
                    Clipboard.SetText(errorMessage);
                    break;
                }
            }
            reader.Close();

        }
        
        public static string Inject(string injectToString, int positionToInject, string stringToInject)
        {
            var builder = new StringBuilder();
            builder.Append(injectToString.Substring(0, positionToInject));
            builder.Append(stringToInject);
            builder.Append(injectToString.Substring(positionToInject));
            return builder.ToString();
        }


        private void SpAtDesignTime_Execute(ExecuteEventArgs ea)
        {
            IEnumerable<ProjectItem> enumerable = CodeRush.ApplicationObject.Solution.FindStartUpProject().ProjectItems.Cast<ProjectItem>();
            Trace.Listeners.Add(new DefaultTraceListener { LogFileName = "log.txt" });
            foreach (ProjectItem item in enumerable) {
                if (item.Name.ToLower() == "app.config" || item.Name.ToLower() == "web.config") {
                    Trace.WriteLine("config found");
                    using (var storage = new DecoupledStorage(typeof (Options))) {
                        foreach (var connectionString in Options.GetConnectionStrings(storage)){
                            if (!string.IsNullOrEmpty(connectionString.Name)){
                                Trace.WriteLine("conneection string found in storage");
                                ConnectionStringSettings connectionStringSettings = GetConnectionStringSettings(item,connectionString.Name);
                                if (connectionStringSettings != null) {
                                    Trace.WriteLine("conneection string found in config");
                                    dropDatabase(connectionStringSettings.ConnectionString,connectionString.Name);
                                }
                            }    
                        }
                    }
                }
            }            
        }

        private ConnectionStringSettings GetConnectionStringSettings(ProjectItem item, string name)
        {
            Property property = item.FindProperty(ProjectItemProperty.FullPath);
            var exeConfigurationFileMap = new ExeConfigurationFileMap { ExeConfigFilename = property.Value.ToString() };
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
            ConnectionStringsSection strings = configuration.ConnectionStrings;
            return strings.ConnectionStrings[name];
        }

        private void dropDatabase(string connectionString, string name)
        {
            string error = null;
            IDataStore provider;
            string database = name;
            try {
                provider = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.None);
                if (provider is MSSqlConnectionProvider)
                    database = dropSqlServerDatabase(connectionString);
                else if (provider is AccessConnectionProvider)
                {
                    database = ((AccessConnectionProvider)provider).Connection.Database;
                    File.Delete(database);
                }
                else
                {
                    throw new NotImplementedException(provider.GetType().FullName);
                }
            }
            catch (UnableToOpenDatabaseException) {
                error = "UnableToOpenDatabase " + database;
            }
            catch (Exception e) {
                Trace.WriteLine(e.ToString());
                error = database+ " Error check log";
            }
            actionHint1.Text = error??database+" DataBase Dropped !!!";
            Rectangle rectangle = Screen.PrimaryScreen.Bounds;
            actionHint1.PointTo(new Point(rectangle.Width / 2, rectangle.Height / 2));
        }

        private string dropSqlServerDatabase(string connectionString) {
            using (var connection = new SqlConnection(connectionString)){
                using (var sqlConnection = new SqlConnection(connectionString.Replace(connection.Database, "master")+";Pooling=false")){
                    sqlConnection.Open();
                    Trace.WriteLine("master db opened");
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "ALTER DATABASE $TargetDataBase$ SET SINGLE_USER WITH ROLLBACK IMMEDIATE".Replace("$TargetDataBase$", connection.Database);
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.CommandText = "DROP DATABASE $TargetDataBase$".Replace("$TargetDataBase$", connection.Database);
                    sqlCommand.ExecuteNonQuery();
                    
                }
                return connection.Database;
            }
        }




    }
}