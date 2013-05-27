using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.Diagnostics.Commands;
using DevExpress.CodeRush.PlugInCore;
using DevExpress.CodeRush.StructuralParser;
using DevExpress.DXCore.Controls.Xpo.DB.Exceptions;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using EnvDTE;
using XpandAddIns;
using XpandAddIns.Enums;
using XpandAddIns.Extensions;
using AutoCreateOption = DevExpress.Xpo.DB.AutoCreateOption;
using Configuration = System.Configuration.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using ConfigurationProperty = XpandAddIns.Enums.ConfigurationProperty;
using Process = System.Diagnostics.Process;
using Project = EnvDTE.Project;
using Property = EnvDTE.Property;

namespace XpandAddins {
    public partial class PlugIn1 : StandardPlugIn {


        private void convertProject_Execute(ExecuteEventArgs ea) {
            string path = Options.ReadString(Options.ProjectConverterPath);
            string token = Options.ReadString(Options.Token);
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(token)) {
                var directoryName = Path.GetDirectoryName(CodeRush.Solution.Active.FileName);
                _actionHint.Text = "Project Converter Started !!!";
                var position = Cursor.Position;
                Rectangle rectangle = Screen.FromPoint(position).Bounds;
                _actionHint.PointTo(new Point(rectangle.Width / 2, rectangle.Height / 2));
                var userName = string.Format("/s /k:{0} \"{1}\"", token, directoryName);
                Process.Start(path, userName);
                
            }
        }

        private void collapseAllItemsInSolutionExplorer_Execute(ExecuteEventArgs ea) {
            CodeRush.ApplicationObject.Solution.CollapseAllFolders();
        }

        private void exploreXafErrors_Execute(ExecuteEventArgs ea) {
            Project startUpProject = CodeRush.ApplicationObject.Solution.FindStartUpProject();
            Property outPut = startUpProject.ConfigurationManager.ActiveConfiguration.FindProperty(ConfigurationProperty.OutputPath);
            bool isWeb = IsWeb(startUpProject);
            string fullPath = startUpProject.FindProperty(ProjectProperty.FullPath).Value + "";
            string path = Path.Combine(fullPath, outPut.Value.ToString()) + "";
            if (isWeb)
                path = Path.GetDirectoryName(startUpProject.FullName);
            Func<Stream> streamSource = () => {
                var path1 = path + "";
                File.Copy(Path.Combine(path1, "expressAppFrameWork.log"), Path.Combine(path1, "expressAppFrameWork.locked"), true);
                return File.Open(Path.Combine(path1, "expressAppFrameWork.locked"), FileMode.Open, FileAccess.Read, FileShare.Read);
            };
            var reader = new ReverseLineReader(streamSource);
            var stackTrace = new List<string>();
            foreach (var readline in reader) {
                stackTrace.Add(readline);
                if (readline.Trim().StartsWith("The error occured:") || readline.Trim().StartsWith("The error occurred:")) {
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
        }

        bool IsWeb(Project startUpProject) {
            return startUpProject.ProjectItems.OfType<ProjectItem>().Any(item => item.Name.ToLower() == "web.config");
        }



        private void SpAtDesignTime_Execute(ExecuteEventArgs ea) {
            IEnumerable<ProjectItem> enumerable = CodeRush.ApplicationObject.Solution.FindStartUpProject().ProjectItems.Cast<ProjectItem>();
            Trace.Listeners.Add(new DefaultTraceListener { LogFileName = "log.txt" });
            foreach (ProjectItem item in enumerable) {
                if (item.Name.ToLower() == "app.config" || item.Name.ToLower() == "web.config") {
                    foreach (var connectionString in Options.GetConnectionStrings()) {
                        if (!string.IsNullOrEmpty(connectionString.Name)) {
                            ConnectionStringSettings connectionStringSettings = GetConnectionStringSettings(item, connectionString.Name);
                            if (connectionStringSettings != null) {
                                DropDatabase(connectionStringSettings.ConnectionString, connectionString.Name);
                            }
                        }
                    }

                }
            }
        }

        private ConnectionStringSettings GetConnectionStringSettings(ProjectItem item, string name) {
            Property property = item.FindProperty(ProjectItemProperty.FullPath);
            var exeConfigurationFileMap = new ExeConfigurationFileMap { ExeConfigFilename = property.Value.ToString() };
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
            ConnectionStringsSection strings = configuration.ConnectionStrings;
            return strings.ConnectionStrings[name];
        }

        private void DropDatabase(string connectionString, string name) {
            string error = null;
            string database = name;
            try {
                var provider = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.None);
                var sqlConnectionProvider = provider as MSSqlConnectionProvider;
                if (sqlConnectionProvider != null) {
                    DropSqlServerDatabase(connectionString);
                    database = sqlConnectionProvider.Connection.Database;
                } else {
                    var connectionProvider = provider as AccessConnectionProvider;
                    if (connectionProvider != null) {
                        database = connectionProvider.Connection.Database;
                        File.Delete(database);
                    } else {
                        throw new NotImplementedException(provider.GetType().FullName);
                    }
                }
            } catch (UnableToOpenDatabaseException) {
                error = "UnableToOpenDatabase " + database;
            } catch (Exception e) {
                Trace.WriteLine(e.ToString());
                error = database + " Error check log";
            }
            _actionHint.Text = error ?? database + " DataBase Dropped !!!";
            Rectangle rectangle = Screen.PrimaryScreen.Bounds;
            _actionHint.PointTo(new Point(rectangle.Width / 2, rectangle.Height / 2));
        }

        private static void DropSqlServerDatabase(string connectionString) {
            var connectionProvider = (MSSqlConnectionProvider)XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.None);
            using (var dbConnection = connectionProvider.Connection) {
                using (var sqlConnection = (SqlConnection)DataStore(connectionString).Connection) {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", dbConnection.Database);
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.CommandText = string.Format("DROP DATABASE {0}", dbConnection.Database);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
        static MSSqlConnectionProvider DataStore(string connectionString) {
            var connectionStringParser = new ConnectionStringParser(connectionString);
            var userid = connectionStringParser.GetPartByName("UserId");
            var password = connectionStringParser.GetPartByName("password");
            string connectionStr;
            if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(password))
                connectionStr = MSSqlConnectionProvider.GetConnectionString(connectionStringParser.GetPartByName("Data Source"), userid, password, "master");
            else {
                connectionStr = MSSqlConnectionProvider.GetConnectionString(connectionStringParser.GetPartByName("Data Source"), "master");
            }
            return (MSSqlConnectionProvider)XpoDefault.GetConnectionProvider(connectionStr, AutoCreateOption.None);
        }

        private void loadProjects_Execute(ExecuteEventArgs ea) {
            var dte = CodeRush.ApplicationObject;
            var UIHSolutionExplorer = dte.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
            if (UIHSolutionExplorer == null || UIHSolutionExplorer.UIHierarchyItems.Count == 0)
                return;
            var uiHierarchyItem = UIHSolutionExplorer.UIHierarchyItems.Item(1);
            
            string constants = Constants.vsext_wk_SProjectWindow;
            if (ea.Action.ParentMenu == "Object Browser Objects Pane")
                constants = Constants.vsWindowKindObjectBrowser;
            Project dteProject = FindProject(uiHierarchyItem);
            ProjectElement activeProject = CodeRush.Language.LoadProject(dteProject);
            if (activeProject != null) {
                var projectLoader = new ProjectLoader();
                var selectedAssemblyReferences = activeProject.GetSelectedAssemblyReferences(constants).ToList();
                projectLoader.Load(selectedAssemblyReferences.ToList(), NotifyOnNotFound);
            } else {
                throw new NotImplementedException();
            }
        }

        void NotifyOnNotFound(string s) {
            _actionHint.Text = "Assembly not found " + s;
            Rectangle rectangle = Screen.PrimaryScreen.Bounds;
            _actionHint.PointTo(new Point(rectangle.Width/2, rectangle.Height/2));
        }

        Project FindProject(UIHierarchyItem uiHierarchyItem, Project project=null) {
            var proj = project;
            foreach (UIHierarchyItem hierarchyItem in uiHierarchyItem.UIHierarchyItems) {
                var findProject = proj ?? hierarchyItem.Object as Project;
                if (hierarchyItem.UIHierarchyItems.Count > 0) {
                    if (hierarchyItem.UIHierarchyItems.Expanded&&FindProject(hierarchyItem, findProject)!=null)
                        return findProject;
                }
                else if (hierarchyItem.IsSelected)
                    return findProject;
            }
            throw new NotImplementedException();
        }

        private void events_ProjectBuildDone(string project, string projectConfiguration, string platform, string solutionConfiguration, bool succeeded) {

            if (succeeded) {
                string gacUtilPath = Options.Storage.ReadString(Options.GetPageName(), Options.GacUtilPath);
                if (File.Exists(gacUtilPath)) {
                    Project dteProject = CodeRush.Solution.Active.FindProjectFromUniqueName(project);
                    if (ProjectExists(dteProject)) {
                        Environment.CurrentDirectory = Path.GetDirectoryName(gacUtilPath) + "";
                        string outputPath = dteProject.FindOutputPath();
                        if (File.Exists(outputPath))
                            Process.Start("gacutil.exe", String.Format(@"/i ""{0}"" /f", outputPath));
                    } else {
                        Log.Send("GagUtl Project Not Found:", dteProject.FileName);
                    }
                }
            }
        }

        public static bool ProjectExists(Project project) {
            IEnumerable<string> allProjectPaths =
                Options.Storage.GetGroupedKeys(Options.ProjectPaths).SelectMany(
                    s => Options.Storage.ReadStrings(Options.ProjectPaths, s));
            return allProjectPaths.Where(MatchProjectName(project)).FirstOrDefault() != null;
        }

        static Func<string, bool> MatchProjectName(Project project) {
            string fileName = Path.GetFileName(project.FileName) + "";
            string pattern = Options.ReadString(Options.GacUtilRegex);
            return s => {
                string s1 = s.Split('|')[0];
                return s1 == project.FileName && (!string.IsNullOrEmpty(pattern) && !Regex.IsMatch(fileName, pattern));
            };
        }




    }
}