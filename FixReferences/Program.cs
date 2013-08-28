using System;
using System.Collections.Generic;
using System.IO;

namespace FixReferences {
    class Program {
        static readonly HashSet<string> _excludedDirs=new HashSet<string>{"DXBuildGenerator"}; 
        static void Main() {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            Execute(Path.GetFullPath(@"..\..\.."));
        }

        public static bool Execute(string rootDir) {
            var files = Directory.GetFiles(rootDir, "*.csproj", SearchOption.AllDirectories);
            var documentHelper = new DocumentHelper();
            foreach (var file in files) {
                var name = Path.GetDirectoryName(file)+"";
                var directoryName = name.Substring(name.LastIndexOf(@"\", StringComparison.Ordinal)+1);
                if (!_excludedDirs.Contains(directoryName)) {
                    var projectReferencesUpdater = new ProjectReferencesUpdater(documentHelper,rootDir);
                    projectReferencesUpdater.Update(file);
                }
            }
            return true;
        }
    }
}
