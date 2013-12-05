using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FixReferences {
    class Program {
        static readonly HashSet<string> _excludedDirs=new HashSet<string>{"DXBuildGenerator"}; 
        static void Main() {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            Execute(Path.GetFullPath(@"..\..\.."));
        }

        public static bool Execute(string rootDir) {
            var documentHelper = new DocumentHelper();
            var files = Directory.GetFiles(rootDir, "*.csproj", SearchOption.AllDirectories);
            foreach (var file in files) {
                var name = Path.GetDirectoryName(file)+"";
                var directoryName = name.Substring(name.LastIndexOf(@"\", StringComparison.Ordinal)+1);
                if (!_excludedDirs.Contains(directoryName)) {
                    var projectReferencesUpdater = new ProjectUpdater(documentHelper,rootDir);
                    projectReferencesUpdater.Update(file);
                }
            }

            var version = GetVersion(rootDir);
            files = Directory.GetFiles(Path.Combine(rootDir, @"Resource\Nuget"), "*.nuspec");
            foreach (var file in files) {
                var projectReferencesUpdater = new NugetUpdater(documentHelper, rootDir, version);
                projectReferencesUpdater.Update(file);
            }

            return true;
        }

        static string GetVersion(string rootDir) {
            using (var fileStream = File.OpenRead(Path.Combine(rootDir, @"Xpand\Xpand.Utils\Properties\AssemblyInfo.cs"))) {
                using (var streamReader = new StreamReader(fileStream)) {
                    return Regex.Match(streamReader.ReadToEnd(), "Version = \"(?<version>[^\"]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase)
                                .Groups["version"].Value;
                }
            }
        }
    }
}
