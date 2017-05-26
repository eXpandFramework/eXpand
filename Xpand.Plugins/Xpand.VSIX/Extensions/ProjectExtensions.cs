using System;
using System.IO;
using System.Linq;
using EnvDTE;
using VSLangProj;

namespace Xpand.VSIX.Extensions{
    public enum ProjectProperty {
        FullPath,
        OutputFileName,
        ProjectType,
        OutputType
    }
    public enum Platform {
        Agnostic,
        Win,
        Web
    }
    public enum Language {
        Unknown,
        CSharp,
        VisualBasic
    }

    public static class ProjectExtensions {
        public static string CommandSeperator(this Project project){
            return project.Language() == Extensions.Language.CSharp ? ";" : "";
        }

        public static string Self(this Project project) {
            return project.Language() == Extensions.Language.CSharp ? "this" : "Me";
        }

        public static string TypeofFunction(this Project project) {
            return project.Language() == Extensions.Language.CSharp ? "typeof" : "GetType";
        }

        public static Language Language(this Project project){
            var extension = Path.GetExtension(project.FileName)+"";
            if (extension.EndsWith("csproj"))
                return Extensions.Language.CSharp;
            if (extension.EndsWith("vbproj"))
                return Extensions.Language.VisualBasic;
            return Extensions.Language.Unknown;
        }

        public static string FileExtension(this Project project){
            return project.Language() == Extensions.Language.CSharp? "cs": "vb";
        }

        private static bool References(Project project, string assemblyName) {
            return ((VSProject)project.Object).References.Cast<Reference>().Any(reference => reference.Name.Contains(assemblyName));
        }

        public static Platform GetPlatform(this Project project) {
            return References(project, "DevExpress.ExpressApp.Win")? Platform.Win : (References(project, "DevExpress.ExpressApp.Web") ? Platform.Web : Platform.Agnostic);
        }

        public static bool HasAuthentication(this Project project){
            string fileName = "WinApplication.Designer";
            if (project.IsWeb()){
                fileName = "WebApplication";
            }
            var path = Path.Combine(Path.GetDirectoryName(project.FullName) + "", $"{fileName}.{project.FileExtension()}");
            return File.Exists(path)&& File.ReadAllLines(path).Any(s => s.Contains("Authentication"));
        }

        public static bool IsApplicationProject(this Project project) {
            string outputFileName = (string)project.FindProperty(ProjectProperty.OutputFileName).Value;
            bool isWin = (Path.GetExtension(outputFileName) + "").EndsWith("exe");
            return project.IsWeb() || isWin;
        }

        public static bool IsWeb(this Project project) {
            var webconfig = Path.Combine(Path.GetDirectoryName(project.FullName)+"","web.config");
            return File.Exists(webconfig);
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