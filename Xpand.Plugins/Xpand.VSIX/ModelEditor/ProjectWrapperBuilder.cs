using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xpand.VSIX.Extensions;
using Project = EnvDTE.Project;
using ProjectItem = Microsoft.Build.Evaluation.ProjectItem;

namespace Xpand.VSIX.ModelEditor {
    public class ProjectWrapperBuilder {
        static IEnumerable<Project> GetProjects() {
            return DteExtensions.DTE.Solution.Projects().Where(project => project.ConfigurationManager != null && project.ProjectItems != null);
        }

        public static IEnumerable<ProjectItemWrapper> GetProjectItemWrappers() {
            return GetProjectItemWrappers(GetProjects());
        }
        static bool IsWeb(string fullPath) {
            var webconfig = Path.Combine(Path.GetDirectoryName(fullPath) + "", "web.config");
            return File.Exists(webconfig);
        }

        public static IEnumerable<ProjectItemWrapper> GetProjectItemWrappers(IEnumerable<Project> projects) {
            var msbuildProjects = GetProjects().Select(project1 => new Microsoft.Build.Evaluation.Project(project1.FullName)).ToList();
            var items = msbuildProjects.SelectMany(project
                => new[] { "None", "Content", "EmbeddedResource" }.SelectMany(project.GetItems)
                    .Where(item => item.EvaluatedInclude.EndsWith(".xafml"))).Select(item => {
                var outputFileName = item.Project.AllEvaluatedProperties.First(property => property.Name == "TargetFileName").EvaluatedValue;
                var fullPath = GetEvaluatedValue(item, "ProjectDir");
                return new ProjectItemWrapper{
                    Name = GetName(item),
                    ModelFileName = Path.GetFileName(item.EvaluatedInclude),
                    OutputPath = GetEvaluatedValue(item, "OutputPath"),
                    OutputFileName = outputFileName,
                    IsApplicationProject = Path.GetExtension(outputFileName) == "exe" || IsWeb(fullPath),
                    FullPath = fullPath,
                    UniqueName =Path.GetDirectoryName(fullPath) + @"\" +GetEvaluatedValue(item, "ProjectFileName"),
                    LocalPath = Path.GetDirectoryName(fullPath) + @"\" + item.EvaluatedInclude
                };
            }).ToArray();
            var localizationModels = items.Where(item =>{
                var match = Regex.Match(item.ModelFileName, @"\A(.*)_(.*)\.xafml\z");
                if (match.Success && item.ModelFileName.EndsWith(match.Groups[2].Value+".xafml"))
                    return items.Any(wrapper => wrapper.ModelFileName == match.Groups[1].Value + ".xafml");
                return false;
            }).ToArray();
            return items.Except(localizationModels);
        }

        private static string GetEvaluatedValue(ProjectItem item, string projectdir){
            return item.Project.AllEvaluatedProperties.First(property => property.Name == projectdir).EvaluatedValue;
        }


        static string GetName(ProjectItem item) {
            return item.EvaluatedInclude == "Model.DesignedDiffs.xafml" ? Path.GetFileNameWithoutExtension(item.Project.FullPath) : Path.GetFileNameWithoutExtension(item.Project.FullPath) + " / " + Path.GetFileNameWithoutExtension(item.EvaluatedInclude);
        }
    }
}