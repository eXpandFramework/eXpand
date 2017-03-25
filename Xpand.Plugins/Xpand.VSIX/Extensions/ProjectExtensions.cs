using System;
using System.IO;
using System.Linq;
using EnvDTE;

namespace Xpand.VSIX.Extensions{
    public enum ProjectProperty {
        FullPath,
        OutputFileName,
        ProjectType,
        OutputType
    }

    public static class ProjectExtensions {
        public static bool IsApplicationProject(this Project project) {
            string outputFileName = (string)project.FindProperty(ProjectProperty.OutputFileName).Value;
            bool isWin = (Path.GetExtension(outputFileName) + "").EndsWith("exe");
            return project.IsWeb() || isWin;
        }

        public static bool IsWeb(this Project startUpProject) {
            return startUpProject.ProjectItems.OfType<ProjectItem>().Any(item => item.Name.ToLower() == "web.config");
        }

        public static string FindOutputPath(this Project project) {
            var path2 = project.ConfigurationManager.ActiveConfiguration.FindProperty(ConfigurationProperty.OutputPath).Value + "";
            if (!Path.IsPathRooted(path2)) {
                string currentDirectory = Environment.CurrentDirectory;
                string directoryName = Path.GetDirectoryName(project.FileName) + "";
                Environment.CurrentDirectory = directoryName;
                string fullPath = Path.GetFullPath(path2);
                Environment.CurrentDirectory = currentDirectory;
                return Path.Combine(fullPath, project.FindProperty(ProjectProperty.OutputFileName).Value + "");
            }
            return path2;
        }

        public static Property FindProperty(this Project project, ProjectProperty projectProperty) {
            string projectPropertyStr = projectProperty.ToString();
            return project.Properties.Cast<Property>().Single(property => property.Name == projectPropertyStr);
        }

    }
}