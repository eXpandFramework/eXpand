using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xpand.VSIX.Extensions;
using ProjectItem = Microsoft.Build.Evaluation.ProjectItem;

namespace Xpand.VSIX.ToolWindow.ModelEditor {
    public static class ProjectWrapperBuilder {
        static bool IsWeb(string fullPath) {
            var webConfig = Path.Combine(Path.GetDirectoryName(fullPath) + "", "web.config");
            return File.Exists(webConfig);
        }

        public static IEnumerable<ProjectItemWrapper> GetProjectItemWrappers(bool models=true) {
            if (models) {
                var items = DteExtensions.DTE.Solution.GetMsBuildProjects()
                    .SelectMany(project => new[] { "None", "Content", "EmbeddedResource" }
                        .SelectMany(project.GetItems)
                    .Where(item => item.EvaluatedInclude.EndsWith(".xafml")))
                    .Select(CreateProjectItemWrapper).ToArray();
                var localizationModels = items.Where(item => FilterLocalizedItems(item, items));
                return items.Except(localizationModels);
            }

            return Options.OptionClass.Instance.SourceCodeInfos.SelectMany(info => info.ProjectPaths).Select(info =>
                new ProjectItemWrapper() {Name = Path.GetFileNameWithoutExtension(info.Path), FullPath = info.Path,LocalPath = info.Path});
        }

        

        private static bool FilterLocalizedItems(ProjectItemWrapper item, ProjectItemWrapper[] items){
            var match = Regex.Match(item.ModelFileName, @"\A(.*)_(.*)\.xafml\z");
            if (match.Success && item.ModelFileName.EndsWith(match.Groups[2].Value + ".xafml"))
                return items.Any(wrapper => wrapper.ModelFileName == match.Groups[1].Value + ".xafml");
            return item.ModelFileName.Contains("Model.DesignedDiffs.Localization.");
        }

        private static ProjectItemWrapper CreateProjectItemWrapper(ProjectItem item){
            var targetFileName = item.Project.AllEvaluatedProperties.First(property => property.Name == "TargetFileName")
                .EvaluatedValue;
            var appendTargetFrameworkToOutputPath = item.Project.AllEvaluatedProperties.FirstOrDefault(property => property.Name == "AppendTargetFrameworkToOutputPath")
                ?.EvaluatedValue;
            var outputPath = GetEvaluatedValue(item, "OutputPath");
            var targetFramework = item.Project.AllEvaluatedProperties.FirstOrDefault(property => property.Name == "TargetFramework")?.EvaluatedValue;
            if (appendTargetFrameworkToOutputPath == "true") {
                outputPath += targetFramework;
            }
            var fullPath = GetEvaluatedValue(item, "ProjectDir");

            var projectItemWrapper = new ProjectItemWrapper {
                Name = GetName(item),
                ModelFileName = Path.GetFileName(item.EvaluatedInclude),
                OutputPath = outputPath,
                OutputFileName = targetFileName,
                FullPath = fullPath,
                UniqueName = Path.GetDirectoryName(fullPath) + @"\" + GetEvaluatedValue(item, "ProjectFileName"),
                LocalPath = Path.GetDirectoryName(fullPath) + @"\" + item.EvaluatedInclude,
                TargetFramework = targetFramework,
                IsApplicationProject =
                    File.Exists(
                        $"{Path.GetFullPath($"{fullPath}")}\\{outputPath}\\{Path.GetFileNameWithoutExtension(targetFileName)}.exe")||IsWeb(fullPath)
            };

            return projectItemWrapper;
        }

        private static string GetEvaluatedValue(ProjectItem item, string projectDir) 
            => item.Project.AllEvaluatedProperties.First(property => property.Name == projectDir).EvaluatedValue;

        static string GetName(this ProjectItem item) => item.EvaluatedInclude == "Model.DesignedDiffs.xafml"
            ? Path.GetFileNameWithoutExtension(item.Project.FullPath)
            : Path.GetFileNameWithoutExtension(item.Project.FullPath) + " / " +
              Path.GetFileNameWithoutExtension(item.EvaluatedInclude);
    }
}