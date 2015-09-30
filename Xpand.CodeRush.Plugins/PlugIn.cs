using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.PlugInCore;
using DevExpress.CodeRush.StructuralParser;
using DevExpress.DXCore.Controls.Xpo.DB.Exceptions;
using DevExpress.DXCore.Controls.Xpo.DB.Helpers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using EnvDTE;
using Xpand.CodeRush.Plugins.Enums;
using Xpand.CodeRush.Plugins.Extensions;
using Xpand.CodeRush.Plugins.ModelEditor;
using XpandPlugins;
using Configuration = System.Configuration.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using ConfigurationProperty = Xpand.CodeRush.Plugins.Enums.ConfigurationProperty;
using Process = System.Diagnostics.Process;
using Project = EnvDTE.Project;
using Property = EnvDTE.Property;
using VSProject = VSLangProj.VSProject;

namespace Xpand.CodeRush.Plugins {
    public partial class PlugIn : StandardPlugIn {
        private bool _lastBuildSucceeded;
        readonly EasyTest _easyTest=new EasyTest();
        public PlugIn(){
            InitializeComponent();
        }

        public override void InitializePlugIn(){
            base.InitializePlugIn();
            _easyTest.CreateButtons();
            _easyTest.QueryLastBuildStatus += (sender, args) => args.Successed = _lastBuildSucceeded;
        }

        private void convertProject_Execute(ExecuteEventArgs ea) {
            string path = Options.ReadString(Options.ProjectConverterPath);
            string token = Options.ReadString(Options.Token);
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(token)) {
                var directoryName = Path.GetDirectoryName(DevExpress.CodeRush.Core.CodeRush.Solution.Active.FileName);
                _actionHint.Text = "Project Converter Started !!!";
                var position = Cursor.Position;
                Rectangle rectangle = Screen.FromPoint(position).Bounds;
                _actionHint.PointTo(new Point(rectangle.Width / 2, rectangle.Height / 2));
                var userName = string.Format("/sc /k:{0} \"{1}\"", token, directoryName);
                Process.Start(path, userName);
            }
        }

        private void collapseAllItemsInSolutionExplorer_Execute(ExecuteEventArgs ea) {
            DevExpress.CodeRush.Core.CodeRush.ApplicationObject.Solution.CollapseAllFolders();
        }

        private void exploreXafErrors_Execute(ExecuteEventArgs ea) {
            Project startUpProject = DevExpress.CodeRush.Core.CodeRush.ApplicationObject.Solution.FindStartUpProject();
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

        private void DropDataBase_Execute(ExecuteEventArgs ea) {
            IEnumerable<ProjectItem> enumerable = DevExpress.CodeRush.Core.CodeRush.ApplicationObject.Solution.FindStartUpProject().ProjectItems.Cast<ProjectItem>();
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
                }
                else {
                    var connectionProvider = provider as AccessConnectionProvider;
                    if (connectionProvider != null) {
                        database = connectionProvider.Connection.Database;
                        File.Delete(database);
                    }
                    else {
                        throw new NotImplementedException(provider.GetType().FullName);
                    }
                }
            }
            catch (UnableToOpenDatabaseException) {
                error = "UnableToOpenDatabase " + database;
            }
            catch (Exception e) {
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
            var dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
            var uihSolutionExplorer = dte.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
            if (uihSolutionExplorer == null || uihSolutionExplorer.UIHierarchyItems.Count == 0)
                return;
            var uiHierarchyItem = uihSolutionExplorer.UIHierarchyItems.Item(1);

            string constants = Constants.vsext_wk_SProjectWindow;
            if (ea.Action.ParentMenu == "Object Browser Objects Pane")
                constants = Constants.vsWindowKindObjectBrowser;
            Project dteProject = FindProject(uiHierarchyItem);
            ProjectElement activeProject = DevExpress.CodeRush.Core.CodeRush.Language.LoadProject(dteProject);
            if (activeProject != null) {
                var projectLoader = new ProjectLoader();
                var selectedAssemblyReferences = activeProject.GetSelectedAssemblyReferences(constants).ToList();
                projectLoader.Load(selectedAssemblyReferences.ToList(), NotifyOnNotFound);
            }
            else {
                throw new NotImplementedException();
            }
        }

        void NotifyOnNotFound(string s) {
            _actionHint.Text = "Assembly not found " + s;
            Rectangle rectangle = Screen.PrimaryScreen.Bounds;
            _actionHint.PointTo(new Point(rectangle.Width / 2, rectangle.Height / 2));
        }

        Project FindProject(UIHierarchyItem uiHierarchyItem, Project project = null) {
            var proj = project;
            foreach (UIHierarchyItem hierarchyItem in uiHierarchyItem.UIHierarchyItems) {
                var findProject = proj ?? hierarchyItem.Object as Project;
                if (hierarchyItem.UIHierarchyItems.Count > 0) {
                    if (hierarchyItem.UIHierarchyItems.Expanded && FindProject(hierarchyItem, findProject) != null)
                        return findProject;
                }
                else if (hierarchyItem.IsSelected)
                    return findProject;
            }
            throw new NotImplementedException();
        }

        private void events_ProjectBuildDone(string project, string projectConfiguration, string platform, string solutionConfiguration, bool succeeded) {
            _lastBuildSucceeded = succeeded;
        }

        private void RunEasyTest_Execute(ExecuteEventArgs ea) {
            _easyTest.RunTest(false);
        }

        private void events_SolutionOpened() {
            if (Options.ReadBool(Options.SpecificVersion)) {
                var dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
                IEnumerable<IFullReference> fullReferences = null;
                Task.Factory.StartNew(() => {
                    fullReferences = GetReferences(dte);
                }).ContinueWith(task => {
                    foreach (var fullReference in fullReferences) {
                        fullReference.SpecificVersion = false;
                    }
                });
            }
        }

        private static IEnumerable<IFullReference> GetReferences(DTE dte) {
            return dte.Solution.Projects.OfType<Project>().SelectMany(project => ((VSProject)project.Object).References.Cast<IFullReference>()).Where(reference =>
                reference.SpecificVersion && (reference.Identity.StartsWith("Xpand") || reference.Identity.StartsWith("DevExpress"))).ToArray();
        }

        private void DebugEasyTest_Execute(ExecuteEventArgs ea) {
            _easyTest.RunTest(true);
        }

        private void PlugIn_DocumentActivated(DocumentEventArgs ea) {
            var validExtensions = new[] { ".ets", ".inc" };
            _easyTest.ChangeButtonsEnableState(validExtensions.Contains(Path.GetExtension(ea.Document.FullName)));
        }
    }
}