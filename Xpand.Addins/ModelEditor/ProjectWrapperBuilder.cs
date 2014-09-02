﻿using System.Collections.Generic;
using System.Linq;
using DevExpress.CodeRush.Core;
using EnvDTE;
using XpandAddIns.Enums;
using XpandAddIns.Extensions;
using Project = EnvDTE.Project;

namespace XpandAddIns.ModelEditor {
    public class ProjectWrapperBuilder {
        static IEnumerable<Project> GetProjects() {
            var projects = CodeRush.Solution.AllProjects.Select(project => CodeRush.Solution.FindEnvDTEProject(project.Name));
            return projects.Where(project => project.ConfigurationManager != null && project.ProjectItems != null);
        }

        static ProjectWrapper ProjectWrapperSelector(ProjectItem item1) {
            return new ProjectWrapper {
                Name = GetName(item1),
                OutputPath = item1.ContainingProject.ConfigurationManager.ActiveConfiguration.FindProperty(ConfigurationProperty.OutputPath).Value.ToString(),
                OutPutFileName = item1.ContainingProject.FindProperty(ProjectProperty.OutputFileName).Value.ToString(),
                FullPath = item1.ContainingProject.FindProperty(ProjectProperty.FullPath).Value.ToString(),
                UniqueName = item1.ContainingProject.UniqueName,
                LocalPath = item1.FindProperty(ProjectItemProperty.LocalPath).Value.ToString()
            };
        }

        public static IEnumerable<ProjectWrapper> GetProjectWrappers() {
            var projects = GetProjects().ToList();
            return GetProjectWrappers(projects);
        }

        public static IEnumerable<ProjectWrapper> GetProjectWrappers(IEnumerable<Project> projects) {
            var projectItems = projects.SelectMany(project1 => project1.ProjectItems.OfType<ProjectItem>()).ToList();

            var items = new List<ProjectWrapper>();
            GetAllItems(projectItems, items);
            return items;
        }

        static void GetAllItems(IEnumerable<ProjectItem> projectItems, List<ProjectWrapper> list) {
            foreach (var projectItem in projectItems) {
                string name = projectItem.Name;
                if (name.EndsWith(".xafml") &&  !name.Contains("Localization") && name.IndexOf(" ", System.StringComparison.Ordinal) == -1)
                    list.Add(ProjectWrapperSelector(projectItem));
                GetAllItems(projectItem.ProjectItems.OfType<ProjectItem>(), list);
            }
        }

        static string GetName(ProjectItem item1) {
            return item1.Name == "Model.DesignedDiffs.xafml" ? item1.ContainingProject.Name : item1.ContainingProject.Name + " / " + item1.Name;
        }
    }
}