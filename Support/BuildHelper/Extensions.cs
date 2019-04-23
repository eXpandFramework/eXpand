using System;

namespace BuildHelper {
    class Extensions {
        public static string PathToRoot(string project,string rootDir) {
            string path = null;
            while (project != rootDir) {
                path += @"..\";
                project = project.Substring(0, project.LastIndexOf(@"\", StringComparison.Ordinal));
            }
            return path;
        }
    }
}
