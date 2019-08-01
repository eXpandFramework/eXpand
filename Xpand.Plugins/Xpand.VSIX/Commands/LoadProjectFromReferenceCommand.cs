using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Fasterflect;
using Mono.Cecil;
using VSLangProj;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Xpand.VSIX.VSPackage;
using Task = System.Threading.Tasks.Task;

namespace Xpand.VSIX.Commands {
    public class LoadProjectFromReferenceCommand:VSCommand{
        private LoadProjectFromReferenceCommand(int commandId,string name) :base((sender, args) => LoadProjects(), new CommandID(PackageGuids.guidVSXpandPackageCmdSet, commandId)) {
            this.EnableForAssemblyReferenceSelection();
            var dteCommand = OptionClass.Instance.DteCommands.FirstOrDefault(command => command.Command == name);
            BindCommand(dteCommand);
        }

        public static void Init(){
            var unused = new LoadProjectFromReferenceCommand(PackageIds.cmdidLoadProjectFromreference,nameof(LoadProjectFromReferenceCommand));
            // ReSharper disable once ObjectCreationAsStatement
            new LoadProjectFromReferenceCommand(PackageIds.cmdidLoadProjectFromreferenceTool,null);
        }

        private static readonly DTE2 DTE = DteExtensions.DTE;
        private static void SkipBuild(Project project) {
            var solutionConfigurations = DTE.Solution.SolutionBuild.SolutionConfigurations.Cast<SolutionConfiguration>();
            var solutionContexts = solutionConfigurations.SelectMany(
                solutionConfiguration => solutionConfiguration.SolutionContexts.Cast<SolutionContext>())
                .Where(context => Path.GetFileNameWithoutExtension(context.ProjectName) == project.Name).ToArray();
            foreach (var solutionContext in solutionContexts) {
                solutionContext.ShouldBuild = false;
            }
        }

        private static void ChangeActiveConfiguration(Project project) {
            var solutionConfigurationNames = DTE.Solution.SolutionBuild.SolutionConfigurations.Cast<SolutionConfiguration>()
                            .OrderByDescending(solutionConfiguration => solutionConfiguration == DTE.Solution.SolutionBuild.ActiveConfiguration).
                            ThenByDescending(solutionConfiguration => string.Equals(solutionConfiguration.Name, "debug", StringComparison.CurrentCultureIgnoreCase)).
                            Select(configuration => configuration.Name).ToArray();
            var configurationName = solutionConfigurationNames.First(solutionConfigurationName => project.ConfigurationManager.Cast<Configuration>()
                        .Any(configuration => configuration.ConfigurationName == solutionConfigurationName));
            var solutionContext = DTE.Solution.SolutionBuild.ActiveConfiguration.SolutionContexts.Cast<SolutionContext>().First(context => Path.GetFileNameWithoutExtension(context.ProjectName) == project.Name);
            solutionContext.ConfigurationName = !string.IsNullOrEmpty(OptionClass.Instance.DefaultConfiguration) ? OptionClass.Instance.DefaultConfiguration : configurationName;
            DTE.WriteToOutput(solutionContext.ConfigurationName + " configuration activated");
        }

        static void LoadProjects() {
            DTE.InitOutputCalls("LoadProjects");
            Task.Factory.StartNew(LoadProjectsCore, CancellationToken.None,TaskCreationOptions.None, TaskScheduler.Default);
        }

        private static void LoadProjectsCore(){
            try{
                var uihSolutionExplorer = DTE.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
                if (uihSolutionExplorer == null || uihSolutionExplorer.UIHierarchyItems.Count == 0)
                    throw new Exception("Nothing selected");
                
                object[] objects = uihSolutionExplorer.GetReferences<Reference>( ).Cast<object>().ToArray();
                object[] references = objects;
                if (!objects.Any()) {
                    objects = uihSolutionExplorer.GetReferences<object>( );
                    if (!objects.Any()) {
                        return;
                    }
                    var projectName = (string) objects.First().GetPropertyValue("Project").GetPropertyValue("Name");
                    
                    var vsProject = (VSProject) DteExtensions.DTE.Solution.Projects().First(project => project.Name == projectName).Object;
                
                    references = objects.Select(o => vsProject.References.Cast<object>().First(_ => (string) _.GetPropertyValue("Name")==(string) o.GetPropertyValue("Name"))).ToArray();
                }

                foreach (var reference in references){
                    var path = GetPath(reference);
                    DTE.WriteToOutput($"Looking for ${path}");
                    var projectInfo = OptionClass.Instance.SourceCodeInfos.SelectMany(info => info.ProjectPaths)
                        .FirstOrDefault(
                            info =>
                                string.Equals(info.OutputPath, path, StringComparison.CurrentCultureIgnoreCase) &&
                                AssemblyDefinition.ReadAssembly(info.OutputPath).VersionMatch());
                    var name = GetName(reference);
                    if (projectInfo != null){
                        DTE.WriteToOutput($"{name} found at " + projectInfo.OutputPath);
                        if (
                            DTE.Solution.Projects()
                                .All(project => project.FullName != projectInfo.Path)){
                            var project = DTE.Solution.AddFromFile(Path.GetFullPath(projectInfo.Path));
                            SkipBuild(project);
                            ChangeActiveConfiguration(project);
                        }
                        else{
                            DTE.WriteToOutput(projectInfo.Path + " already loaded");
                        }
                    }
                    else{
                        DTE.WriteToOutput($"{name} not found ");
                    }
                }
            }
            catch (Exception e){
                DTE.WriteToOutput(e.ToString());
            }
        }

        private static string GetName(object obj){
            if (obj is Reference reference) {
                return reference.Name;
            }
            return $"{obj.GetPropertyValue("Name")}";
        }

        private static string GetPath(object obj){
            if (obj is Reference reference) {
                return reference.Path;
            }
            return $"{obj.GetPropertyValue("Path")}";
        }
    }
}
