﻿using System.IO;
using System.Linq;
using EnvDTE;
using Xpand.CodeRush.Plugins.Enums;
using Project = EnvDTE.Project;

namespace Xpand.CodeRush.Plugins.Extensions {
    public static class ProjectExtensions {
        public static string FindOutputPath(this Project project) {
            var path2 = project.ConfigurationManager.ActiveConfiguration.FindProperty(ConfigurationProperty.OutputPath).Value+"";
            if (!Path.IsPathRooted(path2)) {
                string currentDirectory = System.Environment.CurrentDirectory;
                string directoryName = Path.GetDirectoryName(project.FileName)+"";
                System.Environment.CurrentDirectory = directoryName;
                string fullPath = Path.GetFullPath(path2);
                System.Environment.CurrentDirectory = currentDirectory;
                return Path.Combine(fullPath, project.FindProperty(ProjectProperty.OutputFileName).Value+"");
            }
            return path2;
        }

        public static Property FindProperty(this Project project, ProjectProperty projectProperty) {
            string projectPropertyStr = projectProperty.ToString();
            return project.Properties.Cast<Property>().Single(property => property.Name == projectPropertyStr);
        }

    }
}