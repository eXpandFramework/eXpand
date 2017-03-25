using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Mono.Cecil;
using VSLangProj;
using Xpand.VSIX.Options;

namespace Xpand.VSIX.Extensions {
    public static class LoadProjectFromReference{

        private static readonly DTE _dte = DteExtensions.DTE;

        public static Reference[] GetReferences(this UIHierarchyItem[] uiHierarchyItems, Func<Reference, bool> isFiltered) {
            _dte.SuppressUI = true;
            var references = uiHierarchyItems.GetItems<UIHierarchyItem>(item => {
                if (!(item.Object is Reference)) {
                    item.UIHierarchyItems.Expanded = true;
                }
                return item.UIHierarchyItems.Cast<UIHierarchyItem>();
            }).Select(item => item.Object).OfType<Reference>().Where(isFiltered).ToArray();
            _dte.SuppressUI = false;
            return references;
        }

        private static void SkipBuild(Project project) {
            var solutionConfigurations = _dte.Solution.SolutionBuild.SolutionConfigurations.Cast<SolutionConfiguration>();
            var solutionContexts = solutionConfigurations.SelectMany(
                solutionConfiguration => solutionConfiguration.SolutionContexts.Cast<SolutionContext>())
                .Where(context => Path.GetFileNameWithoutExtension(context.ProjectName) == project.Name).ToArray();
            foreach (var solutionContext in solutionContexts) {
                solutionContext.ShouldBuild = false;
            }

        }

        private static void ChangeActiveConfiguration(Project project) {
            var solutionConfigurationNames = _dte.Solution.SolutionBuild.SolutionConfigurations.Cast<SolutionConfiguration>()
                            .OrderByDescending(solutionConfiguration => solutionConfiguration == _dte.Solution.SolutionBuild.ActiveConfiguration).
                            ThenByDescending(solutionConfiguration => solutionConfiguration.Name.ToLower() == "debug").
                            Select(configuration => configuration.Name).ToArray();
            var configurationName = solutionConfigurationNames.First(solutionConfigurationName => project.ConfigurationManager.Cast<Configuration>()
                        .Any(configuration => configuration.ConfigurationName == solutionConfigurationName));
            var solutionContext = _dte.Solution.SolutionBuild.ActiveConfiguration.SolutionContexts.Cast<SolutionContext>().First(context => Path.GetFileNameWithoutExtension(context.ProjectName) == project.Name);
            solutionContext.ConfigurationName = !string.IsNullOrEmpty(OptionClass.Instance.DefaultConfiguration) ? OptionClass.Instance.DefaultConfiguration : configurationName;
            _dte.WriteToOutput(solutionContext.ConfigurationName + " configuration activated");
        }

        public static void LoadProjects() {
            _dte.InitOutputCalls("LoadProjects");
            Task.Factory.StartNew(LoadProjectsCore, CancellationToken.None,TaskCreationOptions.None, TaskScheduler.Default);
        }

        private static void LoadProjectsCore(){
            try{
                var uihSolutionExplorer = _dte.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
                if (uihSolutionExplorer == null || uihSolutionExplorer.UIHierarchyItems.Count == 0)
                    throw new Exception("Nothing selected");
                var references = GetReferences((UIHierarchyItem[]) uihSolutionExplorer.SelectedItems, reference => true);
                foreach (var reference in references){
                    var projectInfo = OptionClass.Instance.SourceCodeInfos.SelectMany(info => info.ProjectPaths)
                        .FirstOrDefault(
                            info =>
                                info.OutputPath.ToLower() == reference.Path.ToLower() &&
                                AssemblyDefinition.ReadAssembly(info.OutputPath).VersionMatch());
                    if (projectInfo != null){
                        _dte.WriteToOutput(reference.Name + " found at " + projectInfo.OutputPath);
                        if (
                            _dte.Solution.Projects.Cast<Project>()
                                .Where(project => project.CodeModel != null)
                                .All(project => project.FullName != projectInfo.Path)){
                            var project = _dte.Solution.AddFromFile(projectInfo.Path);
                            SkipBuild(project);
                            ChangeActiveConfiguration(project);
                        }
                        else{
                            _dte.WriteToOutput(projectInfo.Path + " already loaded");
                        }
                    }
                    else{
                        _dte.WriteToOutput(reference.Name + " not found ");
                    }
                }
            }
            catch (Exception e){
                _dte.WriteToOutput(e.ToString());
            }
        }
    }
}
