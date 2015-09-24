using System.Linq;
using DevExpress.CodeRush.Core;
using EnvDTE;
using Xpand.CodeRush.Plugins.Enums;
using Project = EnvDTE.Project;

namespace Xpand.CodeRush.Plugins.Extensions {
    public static class SolutionExtension {

        public static Project FindProjectFromUniqueName(this Solution solution, string projectName) {
            DevExpress.CodeRush.Core.Project single = DevExpress.CodeRush.Core.CodeRush.Solution.AllProjects.Single(project1 => project1.UniqueName==projectName);
            return DevExpress.CodeRush.Core.CodeRush.Solution.FindEnvDTEProject(single.Name);
        }

        public static Project FindProject(this Solution solution, string projectName) {
            DevExpress.CodeRush.Core.Project single = DevExpress.CodeRush.Core.CodeRush.Solution.AllProjects.Single(project => project.Name == projectName);
            return DevExpress.CodeRush.Core.CodeRush.Solution.FindEnvDTEProject(single.Name);
        }
        public static Property GetProperty(this Solution solution, SolutionProperty solutionProperty) {
            return solution.Properties.Cast<Property>().Single(property => property.Name == solutionProperty.ToString());
        }
        public static void CollapseAllFolders(this Solution solution) {
            var dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
            var uihSolutionExplorer = dte.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
            if (uihSolutionExplorer == null || uihSolutionExplorer.UIHierarchyItems.Count == 0)
                return;
            UIHierarchyItem rootItem = uihSolutionExplorer.UIHierarchyItems.Item(1);
            rootItem.DTE.SuppressUI = true;
            Collapse(rootItem);
            rootItem.Select(vsUISelectionType.vsUISelectionTypeSelect);
            rootItem.DTE.SuppressUI = false;
        }
        public static Project FindStartUpProject(this Solution solution) {
            Property startUpProperty = solution.GetProperty(SolutionProperty.StartupProject);
            return solution.Projects.Cast<Project>().FirstOrDefault(project => project.Name == (string) startUpProperty.Value);
        }

        private static void Collapse(UIHierarchyItem item) {
            foreach (UIHierarchyItem hierarchyItem in item.UIHierarchyItems) {
                if (hierarchyItem.UIHierarchyItems.Count > 0) {
                    Collapse(hierarchyItem);
                    if (hierarchyItem.UIHierarchyItems.Expanded)
                        hierarchyItem.UIHierarchyItems.Expanded = false;
                }
            }
        }
    }
}