using System.IO;
using System.Linq;
using Xpand.VSIX.Extensions;
using Project = Microsoft.Build.Evaluation.Project;

namespace Xpand.VSIX.ToolWindow.ModelEditor {
    public class ProjectItemWrapper {
        public bool IsApplicationProject { get; set; }

        public string Name { get; set; }

        public string GetOutputPath() 
            => Path.GetFullPath($"{Path.GetDirectoryName(OutputFileName)}\\{OutputPath}");
        public string OutputPath { get; set; }
        public string OutputFileName { get; set; }
        public string FullPath { get; set; }

        public string UniqueName { get; set; }

        public string LocalPath { get; set; }
        public string ModelFileName { get; set; }
        public string TargetFramework { get; set; }
        public Project Project => DteExtensions.DTE.Solution.GetMsBuildProjects().First(_ => _.FullPath == UniqueName);
    }
}