using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Xpand.VSIX.Extensions;
using ProjectItem = Microsoft.Build.Evaluation.ProjectItem;

namespace Xpand.VSIX.ModelEditor {
    public class ProjectWrapperBuilder {
        static IEnumerable<Project> GetProjects(){
            var solutionFullName = DteExtensions.DTE.Solution.FullName;
            if (!string.IsNullOrEmpty(solutionFullName)){
                var solutionFile = SolutionFile.Parse(solutionFullName);
                var projects = solutionFile.ProjectsInOrder
                    .Where(projectInSolution => projectInSolution.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
                    .Select(solution => Path.GetFullPath(solution.AbsolutePath)).Select(path => {
                        try{
                            var globalProperties = new Dictionary<string, string>();
                            var configurationName =DteExtensions.DTE.Solution.Projects()
                                    .FirstOrDefault(project1 => GetFullName(project1, path) == 0)?.ConfigurationManager
                                    .ActiveConfiguration.ConfigurationName ?? "Debug";
                            globalProperties.Add("Configuration", configurationName);
                            var projectCollection = new ProjectCollection(globalProperties);
                            return new Project(path, null, null, projectCollection);
                        }
                        catch (Exception e){
                            DteExtensions.DTE.LogError($"Path={path}{Environment.NewLine}{e}");
                            DteExtensions.DTE.WriteToOutput($"Path={path}{Environment.NewLine}{e}");
                            return null;
                        }
                    }).Where(project => project!=null).ToArray();
                return projects;
            }
            return Enumerable.Empty<Project>();
        }

        private static int GetFullName(EnvDTE.Project project1, string path){
            try{
                return string.Compare(project1.FullName,path,StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception){
                return 1;
            }
        }

        static bool IsWeb(string fullPath) {
            var webconfig = Path.Combine(Path.GetDirectoryName(fullPath) + "", "web.config");
            return File.Exists(webconfig);
        }

        public static IEnumerable<ProjectItemWrapper> GetProjectItemWrappers() {
            var items = GetProjects().SelectMany(project => new[] { "None", "Content", "EmbeddedResource" }.SelectMany(project.GetItems)
                .Where(item => item.EvaluatedInclude.EndsWith(".xafml"))).Select(CreateProjectItemWrapper).ToArray();
            var localizationModels = items.Where(item => FilterLocalizedItems(item, items));
            return items.Except(localizationModels);
        }

        private static bool FilterLocalizedItems(ProjectItemWrapper item, ProjectItemWrapper[] items){
            var match = Regex.Match(item.ModelFileName, @"\A(.*)_(.*)\.xafml\z");
            if (match.Success && item.ModelFileName.EndsWith(match.Groups[2].Value + ".xafml"))
                return items.Any(wrapper => wrapper.ModelFileName == match.Groups[1].Value + ".xafml");
            return item.ModelFileName.Contains("Model.DesignedDiffs.Localization.");
        }

        private static ProjectItemWrapper CreateProjectItemWrapper(ProjectItem item){
            var outputFileName = item.Project.AllEvaluatedProperties.First(property => property.Name == "TargetFileName")
                .EvaluatedValue;
            var fullPath = GetEvaluatedValue(item, "ProjectDir");
            return new ProjectItemWrapper{
                Name = GetName(item),
                ModelFileName = Path.GetFileName(item.EvaluatedInclude),
                OutputPath = GetEvaluatedValue(item, "OutputPath"),
                OutputFileName = outputFileName,
                IsApplicationProject = Path.GetExtension(outputFileName) == ".exe" || IsWeb(fullPath),
                FullPath = fullPath,
                UniqueName = Path.GetDirectoryName(fullPath) + @"\" + GetEvaluatedValue(item, "ProjectFileName"),
                LocalPath = Path.GetDirectoryName(fullPath) + @"\" + item.EvaluatedInclude
            };
        }

        private static string GetEvaluatedValue(ProjectItem item, string projectdir){
            return item.Project.AllEvaluatedProperties.First(property => property.Name == projectdir).EvaluatedValue;
        }

        static string GetName(ProjectItem item) {
            return item.EvaluatedInclude == "Model.DesignedDiffs.xafml" ? Path.GetFileNameWithoutExtension(item.Project.FullPath) : Path.GetFileNameWithoutExtension(item.Project.FullPath) + " / " + Path.GetFileNameWithoutExtension(item.EvaluatedInclude);
        }
    }
}