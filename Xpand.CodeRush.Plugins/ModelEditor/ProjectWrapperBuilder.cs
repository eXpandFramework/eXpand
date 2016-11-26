using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Xpand.CodeRush.Plugins.Enums;
using Xpand.CodeRush.Plugins.Extensions;
using Project = EnvDTE.Project;

namespace Xpand.CodeRush.Plugins.ModelEditor {
    public class ProjectWrapperBuilder {
        static IEnumerable<Project> GetProjects() {
            var projects = DevExpress.CodeRush.Core.CodeRush.Solution.AllProjects.Select(project => DevExpress.CodeRush.Core.CodeRush.Solution.FindEnvDTEProject(project.Name));
            return projects.Where(project => project.ConfigurationManager != null && project.ProjectItems != null);
        }

        static ProjectItemWrapper ProjectItemWrapperSelector(ProjectItem item1) {
            return new ProjectItemWrapper {
                Name = GetName(item1),
                OutputPath = item1.ContainingProject.ConfigurationManager.ActiveConfiguration.FindProperty(ConfigurationProperty.OutputPath).Value.ToString(),
                OutPutFileName = item1.ContainingProject.FindProperty(ProjectProperty.OutputFileName).Value.ToString(),
                FullPath = item1.ContainingProject.FindProperty(ProjectProperty.FullPath).Value.ToString(),
                UniqueName = item1.ContainingProject.UniqueName,
                LocalPath = (item1.FindProperty(ProjectItemProperty.LocalPath) ?? item1.FindProperty(ProjectItemProperty.FullPath)).Value.ToString()
            };
        }

        public static IEnumerable<ProjectItemWrapper> GetProjectItemWrappers() {
            var projects = GetProjects().ToList();
            return GetProjectItemWrappers(projects);
        }

        public static IEnumerable<ProjectItemWrapper> GetProjectItemWrappers(IEnumerable<Project> projects) {
            var projectItems = projects.SelectMany(project1 => project1.ProjectItems.OfType<ProjectItem>()).ToList();

            var items = new List<ProjectItemWrapper>();
            GetAllModelItems(projectItems, items);
            return items;
        }

        static void GetAllModelItems(IEnumerable<ProjectItem> projectItems, List<ProjectItemWrapper> list) {
            foreach (var projectItem in projectItems) {
                string name = projectItem.Name;
                if (name.EndsWith(".xafml") &&  !name.Contains("Localization") && name.IndexOf(" ", StringComparison.Ordinal) == -1)
                    list.Add(ProjectItemWrapperSelector(projectItem));
                if (projectItem.ProjectItems != null)
                    GetAllModelItems(projectItem.ProjectItems.OfType<ProjectItem>(), list);
            }
        }

        static string GetName(ProjectItem item1) {
            return item1.Name == "Model.DesignedDiffs.xafml" ? item1.ContainingProject.Name : item1.ContainingProject.Name + " / " + item1.Name;
        }
    }
}