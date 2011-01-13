using System.Linq;
using DevExpress.CodeRush.Core;
using EnvDTE;
using XpandAddIns.Enums;
using Project = EnvDTE.Project;

namespace XpandAddIns.Extensioons {
    public static class SolutionExtension {

        public static Project FindProject(this Solution solution, string projectName) {
            DevExpress.CodeRush.Core.Project single = CodeRush.Solution.AllProjects.Where(project => project.Name == projectName).Single();
            return CodeRush.Solution.FindEnvDTEProject(single.Name);
        }
        public static Property GetProperty(this Solution solution, SolutionProperty solutionProperty) {
            return solution.Properties.Cast<Property>().Where(property => property.Name == solutionProperty.ToString()).Single();
        }
        public static void CollapseAllFolders(this Solution solution) {
            var DTE = CodeRush.ApplicationObject;
            var UIHSolutionExplorer = DTE.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
            if (UIHSolutionExplorer == null || UIHSolutionExplorer.UIHierarchyItems.Count == 0)
                return;
            UIHierarchyItem rootItem = UIHSolutionExplorer.UIHierarchyItems.Item(1);
            rootItem.DTE.SuppressUI = true;
            Collapse(rootItem);
            // Select the solution node, or else when you click 
            // on the solution window 
            // scrollbar, it will synchronize the open document 
            rootItem.Select(vsUISelectionType.vsUISelectionTypeSelect);
            rootItem.DTE.SuppressUI = false;
        }
        public static Project FindStartUpProject(this Solution solution) {
            Property startUpProperty = solution.GetProperty(SolutionProperty.StartupProject);
            return solution.FindProject((startUpProperty.Value + ""));
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