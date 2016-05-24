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
using DevExpress.DXCore.Controls.Xpo.DB.Helpers;
using EnvDTE;
using Mono.Cecil;
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
        }

        public override void InitializePlugIn(){
            base.InitializePlugIn();
            _easyTest.CreateButtons();
            _easyTest.QueryLastBuildStatus += (sender, args) => args.Successed = _lastBuildSucceeded;
        }

        private void convertProject_Execute(ExecuteEventArgs ea) {
            _dte.InitOutputCalls("ConvertProject");
            string path = GetProjectConverterPath();
            string token = OptionClass.Instance.Token;
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(token)) {
                var directoryName = Path.GetDirectoryName(DevExpress.CodeRush.Core.CodeRush.Solution.Active.FileName);
                _dte.WriteToOutput("Project Converter Started !!!");
                var userName = string.Format("/sc /k:{0} \"{1}\"", token, directoryName);
                Process.Start(path, userName);
            }
        }

        private static string GetProjectConverterPath(){
            if (string.IsNullOrWhiteSpace(OptionClass.Instance.ProjectConverterPath)){
                var version = DevExpress.CodeRush.Core.CodeRush.Solution.Active.GetDXVersion();
                var dxRootDirectory = DevExpress.CodeRush.Core.CodeRush.Solution.Active.GetDXRootDirectory();
                return Path.Combine(dxRootDirectory + @"\Tools\Components", "TestExecutor." + version + ".exe");
            }
            return OptionClass.Instance.ProjectConverterPath;
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
            Task.Factory.StartNewNow(() =>{
            
                var startUpProject = _dte.Solution.FindStartUpProject();
                var configItem = startUpProject.ProjectItems.Cast<ProjectItem>()
                    .FirstOrDefault(item => new[] { "app.config", "web.config" }.Contains(item.Name.ToLower()));
                if (configItem==null){
                    _dte.WriteToOutput("Startup project "+startUpProject.Name+" does not contain a config file");
                    return;
                }
                foreach (ConnectionString optionsConnectionString in OptionClass.Instance.ConnectionStrings) {
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
            _dte.WriteToOutput("ConnectionStrings name:" +connectionStringSettings.Name+ " data source: " + parser.GetPartByName("data source"));
            using (var connection = new SqlConnection(parser.GetConnectionString())){
                connection.Open();
                object result;
                using (var sqlCommand = connection.CreateCommand()) {
                    sqlCommand.CommandText = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", database);
                    result = sqlCommand.ExecuteScalar();
                }
                var exists = result != null && int.Parse(result + "") > 0;
                string doesNot = exists ? null : " does not";
                _dte.WriteToOutput("Database "+database+doesNot+" exists");
                return exists;
            }
        }

        private void loadProjects_Execute(ExecuteEventArgs ea) {
            _dte.InitOutputCalls("LoadProjects");
            Task.Factory.StartNewNow(LoadProjects);
        }

        private void LoadProjects(){
            try{
                var uihSolutionExplorer = _dte.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
                if (uihSolutionExplorer == null || uihSolutionExplorer.UIHierarchyItems.Count == 0)
                    throw new Exception("Nothing selected");
                var references = GetReferences(reference => true, (UIHierarchyItem[])uihSolutionExplorer.SelectedItems);
                foreach (var reference in references){
                    var projectInfo = OptionClass.Instance.SourceCodeInfos.SelectMany(info => info.ProjectPaths)
                        .FirstOrDefault(info =>info.OutputPath.ToLower() == reference.Path.ToLower() &&AssemblyDefinition.ReadAssembly(info.OutputPath).VersionMatch());
                    if (projectInfo!=null){
                        _dte.WriteToOutput(reference.Name + " found at "+projectInfo.OutputPath);
                        DevExpress.CodeRush.Core.CodeRush.Solution.Active.AddFromFile(projectInfo.Path);
                        var solutionContexts =_dte.Solution.SolutionBuild.ActiveConfiguration.SolutionContexts.Cast<SolutionContext>()
                                .Where(context =>Path.GetFileName(context.ProjectName) == Path.GetFileName(projectInfo.Path)).ToArray();
                        foreach (var solutionContext in solutionContexts){
                            solutionContext.ShouldBuild = false;
                            solutionContext.ConfigurationName = _dte.Solution.SolutionBuild.ActiveConfiguration.Name;
                        }
                    }
                    else {
                        _dte.WriteToOutput(reference.Name + " not found " );
                    }
                }
            }
            catch (Exception e){
                _dte.WriteToOutput(e.ToString());
            }
        }

        private void events_ProjectBuildDone(string project, string projectConfiguration, string platform, string solutionConfiguration, bool succeeded) {
            _lastBuildSucceeded = succeeded;
        }

        private void RunEasyTest_Execute(ExecuteEventArgs ea) {
            _easyTest.RunTest(false);
        }

        private void events_SolutionOpened() {
            if (OptionClass.Instance.SpecificVersion) {
                var dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
                IEnumerable<IFullReference> fullReferences = null;
                Task.Factory.StartNewNow(() => {
                    fullReferences = dte.GetReferences();
                }).ContinueWith(task => {
                    foreach (var fullReference in fullReferences) {
                        fullReference.SpecificVersion = false;
                    }
                });
            }
        }

        private void DebugEasyTest_Execute(ExecuteEventArgs ea) {
            _easyTest.RunTest(true);
        }

        private void PlugIn_DocumentActivated(DocumentEventArgs ea) {
            var validExtensions = new[] { ".ets", ".inc" };
            _easyTest.ChangeButtonsEnableState(validExtensions.Contains(Path.GetExtension(ea.Document.FullName)));
        }

        private void events_ProjectBuildBegin(string project, string projectConfiguration, string platform, string solutionConfiguration){
            if (OptionClass.Instance.KillModelEditor) {
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

        private void replaceReferencePath_Execute(ExecuteEventArgs ea) {
            _dte.InitOutputCalls("Replace Reference Paths");
            Task.Factory.StartNewNow(() => ModifyReferences(reference => true));
        }

        private IEnumerable<string> LocateAssemblyInFolders(string name, IEnumerable<ReferencedAssembliesFolder> referencedAssembliesFolders){
            foreach (var referencedAssembliesFolder in referencedAssembliesFolders){
                var path = Directory.GetFiles(referencedAssembliesFolder.Folder,"*.dll").FirstOrDefault(s => Path.GetFileNameWithoutExtension(s)==name);
                if (path != null){
                    yield return path;
                }
            }
        }

        private void ModifyReferences(Func<Reference,bool> isFiltered){
            try{
                var referencedAssembliesFolders = OptionClass.Instance.ReferencedAssembliesFolders;
                if (referencedAssembliesFolders.Count == 0)
                    throw new Exception("Use options to add assmbly lookup folders");
                var uihSolutionExplorer = _dte.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
                if (uihSolutionExplorer == null || uihSolutionExplorer.UIHierarchyItems.Count == 0)
                    throw new Exception("Nothing found");
                var uiHierarchyItems = ((UIHierarchyItem[])uihSolutionExplorer.SelectedItems);
                var references = GetReferences(isFiltered, uiHierarchyItems);
                var vsProject = ((VSProject)references.First().ContainingProject.Object);
                _dte.WriteToOutput(references.Length + " references detected");
                foreach (Reference reference in references) {
                    var referenceName = reference.Name;
                    _dte.WriteToOutput("Locating "+referenceName);
                    var assemblyPath =LocateAssemblyInFolders(referenceName,referencedAssembliesFolders).FirstOrDefault(path => AssemblyDefinition.ReadAssembly(path).VersionMatch());
                    if (assemblyPath != null){
                        vsProject.References.Cast<Reference>().First(reference1 => reference1.Name==reference.Name).Remove();
                        vsProject.References.Add(assemblyPath);
                        _dte.WriteToOutput(referenceName + " replaced from " + Path.GetDirectoryName(assemblyPath));
                    }
                    else
                        _dte.WriteToOutput(referenceName + " replacement not found");
                }
            }
            catch (Exception e){
                _dte.WriteToOutput(e.ToString());
            }
        }

        private Reference[] GetReferences(Func<Reference, bool> isFiltered, UIHierarchyItem[] uiHierarchyItems){
            _dte.SuppressUI = true;
            var references = uiHierarchyItems.GetItems<UIHierarchyItem>(item =>{
                if (!(item.Object is Reference)){
                    item.UIHierarchyItems.Expanded = true;
                }
                return item.UIHierarchyItems.Cast<UIHierarchyItem>();
            }).Select(item => item.Object).OfType<Reference>().Where(isFiltered).ToArray();
            _dte.SuppressUI = false;
            return references;
        }

        private void MissingReferences_Execute(ExecuteEventArgs ea) {
            _dte.InitOutputCalls("Fix missing Paths or dx version missmatch");
            Task.Factory.StartNewNow(
                () => ModifyReferences(reference => !File.Exists(reference.Path) || VersionMissMatch(reference.Path)));
        }

        private bool VersionMissMatch(string path){
            return !(Path.GetFileName(path) + "").StartsWith("DevExpress")&&!AssemblyDefinition.ReadAssembly(path).VersionMatch();
        }
    }
}