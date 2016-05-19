using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.PlugInCore;
using DevExpress.CodeRush.StructuralParser;
using DevExpress.DXCore.Controls.Xpo.DB.Helpers;
using EnvDTE;
using Xpand.CodeRush.Plugins.Enums;
using Xpand.CodeRush.Plugins.Extensions;
using Xpand.CodeRush.Plugins.ModelEditor;
using Configuration = System.Configuration.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using ConfigurationProperty = Xpand.CodeRush.Plugins.Enums.ConfigurationProperty;
using Process = System.Diagnostics.Process;
using Project = EnvDTE.Project;
using Property = EnvDTE.Property;
using Reference = VSLangProj.Reference;
using VSProject = VSLangProj.VSProject;

namespace Xpand.CodeRush.Plugins {
    public partial class PlugIn : StandardPlugIn {
        private readonly DTE _dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
        private bool _lastBuildSucceeded;
        readonly EasyTest _easyTest=new EasyTest();
        public PlugIn(){
            InitializeComponent();
            EventNexus.DXCoreLoaded += EventNexusOnDXCoreLoaded;

        }

        private void EventNexusOnDXCoreLoaded(DXCoreLoadedEventArgs ea){
            DevExpress.CodeRush.Core.CodeRush.ToolWindows.Show<METoolWindow>();

        public override void InitializePlugIn(){
            base.InitializePlugIn();
            _easyTest.CreateButtons();
            _easyTest.QueryLastBuildStatus += (sender, args) => args.Successed = _lastBuildSucceeded;
        }

        private void convertProject_Execute(ExecuteEventArgs ea) {
            _dte.InitOutputCalls("ConvertProject");
            string path = Options.ReadString(Options.ProjectConverterPath);
            string token = Options.ReadString(Options.Token);
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(token)) {
                var directoryName = Path.GetDirectoryName(DevExpress.CodeRush.Core.CodeRush.Solution.Active.FileName);
                _dte.WriteToOutput("Project Converter Started !!!");
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

        private void DropDataBase_Execute(ExecuteEventArgs ea){
            _dte.InitOutputCalls("Dropdatabase");
            Task.Factory.StartNew(() =>{
            
                var startUpProject = _dte.Solution.FindStartUpProject();
                var configItem = startUpProject.ProjectItems.Cast<ProjectItem>()
                    .FirstOrDefault(item => new[] { "app.config", "web.config" }.Contains(item.Name.ToLower()));
                if (configItem==null){
                    _dte.WriteToOutput("Startup project "+startUpProject.Name+" does not contain a config file");
                    return;
                }
                foreach (Options.ConnectionString optionsConnectionString in Options.GetConnectionStrings()) {
                    if (!string.IsNullOrEmpty(optionsConnectionString.Name)) {
                        var connectionStringSettings = GetConnectionStringSettings(configItem,optionsConnectionString.Name);
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

            });
        }

        private ConnectionStringSettings GetConnectionStringSettings(ProjectItem item, string name) {
            Property property = item.FindProperty(ProjectItemProperty.FullPath);
            var exeConfigurationFileMap = new ExeConfigurationFileMap { ExeConfigFilename = property.Value.ToString() };
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
            ConnectionStringsSection strings = configuration.ConnectionStrings;
            return strings.ConnectionStrings[name];
        }

        private void DropSqlServerDatabase(ConnectionStringSettings connectionStringSettings) {
            _dte.WriteToOutput("Attempting connection with "+connectionStringSettings.ConnectionString);
            var parser = new ConnectionStringParser(connectionStringSettings.ConnectionString.ToLower());
            parser.RemovePartByName("xpoprovider");
            var database = parser.GetPartByName("initial catalog");
            parser.RemovePartByName("initial catalog");
            using (var connection = new SqlConnection(parser.GetConnectionString())){
                connection.Open();
                using (SqlCommand sqlCommand = connection.CreateCommand()){
                    sqlCommand.CommandText = string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE",database);
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.CommandText = string.Format("DROP DATABASE [{0}]", database);
                    sqlCommand.ExecuteNonQuery();
                    _dte.WriteToOutput(database + " dropped successfully");
                }
            }
        }

        private  bool DbExist(ConnectionStringSettings connectionStringSettings){
            var parser = new ConnectionStringParser(connectionStringSettings.ConnectionString.ToLower());
            var database = parser.GetPartByName("initial catalog");
            parser.RemovePartByName("initial catalog");
            parser.RemovePartByName("xpoprovider");
            _dte.WriteToOutput("Attempting connection with data source " + parser.GetPartByName("data source"));
            using (var connection = new SqlConnection(parser.GetConnectionString())){
                connection.Open();
                object result;
                using (var sqlCommand = connection.CreateCommand()) {
                    sqlCommand.CommandText = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", database);
                    result = sqlCommand.ExecuteScalar();
                }
                var exists = result != null && int.Parse(result + "") > 0;
                string doesNot = exists ? null : " does not";
                _dte.WriteToOutput(database+doesNot+" exists");
                return exists;
            }
        }

        private void loadProjects_Execute(ExecuteEventArgs ea) {
            _dte.InitOutputCalls("LoadProjects");
            var uihSolutionExplorer = _dte.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
            if (uihSolutionExplorer == null || uihSolutionExplorer.UIHierarchyItems.Count == 0)
                return;
            string constants = Constants.vsext_wk_SProjectWindow;
            if (ea.Action.ParentMenu == "Object Browser Objects Pane")
                constants = Constants.vsWindowKindObjectBrowser;
            Task.Factory.StartNew(() => LoadProjects(uihSolutionExplorer, constants));
        }

        private void LoadProjects(UIHierarchy uihSolutionExplorer, string constants){
            var hierarchyItem = ((UIHierarchyItem[]) uihSolutionExplorer.SelectedItems)[0];
            Project dteProject = ((Reference) hierarchyItem.Object).ContainingProject;
            ProjectElement activeProject = DevExpress.CodeRush.Core.CodeRush.Language.LoadProject(dteProject);
            if (activeProject != null){
                var projectLoader = new ProjectLoader();
                var selectedAssemblyReferences = activeProject.GetSelectedAssemblyReferences(constants).ToList();
                _dte.WriteToOutput("Attempting load of " + selectedAssemblyReferences.Count + " selected assemblies from " +
                                   activeProject.Name);
                if (projectLoader.Load(selectedAssemblyReferences)){
                    _dte.WriteToOutput("All projects loaded");
                }
            }
            else{
                throw new NotImplementedException();
            }
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
            return dte.Solution.Projects.OfType<Project>().SelectMany(project => ((VSProject)project.Object).References.OfType<IFullReference>()).Where(reference =>
                reference.SpecificVersion && (reference.Identity.StartsWith("Xpand") || reference.Identity.StartsWith("DevExpress"))).ToArray();
        }

        private void DebugEasyTest_Execute(ExecuteEventArgs ea) {
            _easyTest.RunTest(true);
        }

        private void PlugIn_DocumentActivated(DocumentEventArgs ea) {
            var validExtensions = new[] { ".ets", ".inc" };
            _easyTest.ChangeButtonsEnableState(validExtensions.Contains(Path.GetExtension(ea.Document.FullName)));
        }

        private void events_ProjectBuildBegin(string project, string projectConfiguration, string platform, string solutionConfiguration){
            if (Options.ReadBool(Options.KillModelEditor)) {
                var processes =Process.GetProcesses().Where(process => process.ProcessName.StartsWith("Xpand.ExpressApp.ModelEditor")).ToArray();
                if (processes.Any()){
                    var dialogResult =MessageBox.Show(
                            "The build will probably fail because " + processes.Length +
                            " ModelEditor instance/s is locking the assemblies. Do you want to kill all ModelEditor instances?",
                            "ModelEditor is running", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes){
                        foreach (var process in processes){
                            process.Kill();
                        }
                    }
                }
            }
        }
    }
}