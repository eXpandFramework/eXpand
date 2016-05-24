using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.Win32;
using VSLangProj;
using Xpand.CodeRush.Plugins.Enums;
using Project = EnvDTE.Project;
using Property = EnvDTE.Property;

namespace Xpand.CodeRush.Plugins.Extensions {
    public static class SolutionExtension {
        public static bool VersionMatch(this Assembly assembly) {
            var dxVersion = DevExpress.CodeRush.Core.CodeRush.Solution.Active.GetDXVersion();
            var referencedAssemblies = assembly.GetReferencedAssemblies();
            var dxAssemblies = referencedAssemblies.Where(name => name.Name.StartsWith("DevExpress")).ToArray();
            return !dxAssemblies.Any() || dxAssemblies.Any(name => name.Name.Contains(dxVersion));
        }

        public static string GetDXRootDirectory(this Solution solution){
            var registryKey = Registry.LocalMachine.OpenSubKey(@"Software\WOW6432node\DevExpress\Components\" + solution.GetDXVersion());
            return registryKey != null ? (string) registryKey.GetValue("RootDirectory") : null;
        }

        public static string GetDXVersion(this Solution solution){
            return solution.DTE.Solution.Projects.Cast<Project>()
                .Select(project => project.Object)
                .Cast<VSProject>()
                .SelectMany(project => project.References.Cast<Reference>()).Select(reference =>{
                    var matchResults = Regex.Match(reference.Name, @"DevExpress(.*)(v[^.]*\.[.\d])");
                    return matchResults.Success ? matchResults.Groups[2].Value : null;
                }).FirstOrDefault(version => version!=null);
        }

        public static Project FindProjectFromUniqueName(this Solution solution, string projectName) {
            DevExpress.CodeRush.Core.Project single = DevExpress.CodeRush.Core.CodeRush.Solution.AllProjects.Single(project1 => project1.UniqueName==projectName);
            return DevExpress.CodeRush.Core.CodeRush.Solution.FindEnvDTEProject(single.Name);
        }

        public static Project FindProject(this Solution solution, string projectName) {
            DevExpress.CodeRush.Core.Project single = DevExpress.CodeRush.Core.CodeRush.Solution.AllProjects.Single(project => project.Name == projectName);
            return DevExpress.CodeRush.Core.CodeRush.Solution.FindEnvDTEProject(single.Name);
        }
        public static Property GetProperty(this Solution solution, SolutionProperty solutionProperty) {
            string solutionPropertyStr = solutionProperty.ToString();
            return solution.Properties.Cast<Property>().Single(property => property.Name == solutionPropertyStr);
        }
        public static void CollapseAllFolders(this Solution solution) {
            var dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
            var uihSolutionExplorer = dte.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
            if (uihSolutionExplorer == null || uihSolutionExplorer.UIHierarchyItems.Count == 0)
                return;
            UIHierarchyItem hierarchyItem = uihSolutionExplorer.UIHierarchyItems.Item(1);
            hierarchyItem.DTE.SuppressUI = true;
            hierarchyItem.Expand(false);
            hierarchyItem.Select(vsUISelectionType.vsUISelectionTypeSelect);
            hierarchyItem.DTE.SuppressUI = false;
        }
        public static Project FindStartUpProject(this Solution solution) {
            Property startUpProperty = solution.GetProperty(SolutionProperty.StartupProject);
            return solution.Projects.Cast<Project>().FirstOrDefault(project => project.Name == (string) startUpProperty.Value);
        }

        public static void Expand(this UIHierarchyItem item,bool expand) {
            foreach (UIHierarchyItem hierarchyItem in item.UIHierarchyItems) {
                if (hierarchyItem.UIHierarchyItems.Count > 0) {
                    Expand(hierarchyItem,expand);
                    if (hierarchyItem.UIHierarchyItems.Expanded)
                        hierarchyItem.UIHierarchyItems.Expanded = expand;
                }
            }
        }
    }
}