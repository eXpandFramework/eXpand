using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;

namespace PDBLinker {
    static class Extensions {
        public static IEnumerable<T> TakeAllButLast<T>(this IEnumerable<T> source) {
            var it = source.GetEnumerator();
            bool hasRemainingItems;
            bool isFirst = true;
            T item = default(T);

            do {
                hasRemainingItems = it.MoveNext();
                if (hasRemainingItems) {
                    if (!isFirst) yield return item;
                    item = it.Current;
                    isFirst = false;
                }
            } while (hasRemainingItems);
        }

        public static string GetOutputPdbFile(this Project project) {
            var outputFile = project.GetOutputFile();
            return Path.ChangeExtension(outputFile, ".pdb");
        }

        public static string GetOutputFile(this Project project) {
            string extension = ".dll";
            string outputType = project.GetProperty("OutputType").EvaluatedValue;
            if (outputType.Contains("Exe") || outputType.Contains("WinExe")) {
                extension = ".exe";
            }
            var projectOutputPath = project.GetPropertyValue("OutputPath");
            var outputPath = Path.Combine(project.DirectoryPath, projectOutputPath);
            return Path.Combine(outputPath, string.Format("{0}{1}", project.GetProperty("AssemblyName").EvaluatedValue, extension));
        }

        public static IEnumerable<ProjectItem> GetCompilableItems(this Project project) {
            return project.Items.Where(x => string.Equals(x.ItemType, "Compile"));
        }
    }
}
