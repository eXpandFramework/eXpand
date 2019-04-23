using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;

namespace BuildHelper {
    class Program {
        static readonly HashSet<string> _excludedDirs = new HashSet<string> { "DXBuildGenerator", "Xpand.DesignExperience", "Report designer script editor","RECYCLE.BIN","Archived" };
        private static Options _options;

        static void Main(string[] args) {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var rootDir = Path.GetFullPath(@"..");
            _options = new Options();
            bool arguments = Parser.Default.ParseArguments(args, _options);
            if (arguments){
                Execute(rootDir);
            }
        }

        public static Options Options => _options;

        public static bool Execute(string rootDir){
            DeleteBackupFolders(rootDir);
            var version = GetVersion(rootDir);
            var documentHelper = new DocumentHelper();
            var slnFiles = GetFiles(rootDir, "*.sln").ToArray();
            foreach (var slnFile in slnFiles){
                DebugConfigForEasyTestBuild(slnFile);
            }
            var projectFiles = GetFiles(rootDir, "*.csproj").ToArray();
            foreach (var file in projectFiles) {
                var projectReferencesUpdater = new ProjectUpdater(documentHelper,rootDir,version);
                projectReferencesUpdater.Update(file);
            }

            var nuspecs = Directory.GetFiles(Path.Combine(rootDir, @"Support\Nuspec"), "*.nuspec");
            foreach (var file in nuspecs) {
                var projectReferencesUpdater = new NuspecUpdater(documentHelper, rootDir, version, projectFiles,nuspecs);
                projectReferencesUpdater.Update(file);
            }
            return true;
        }

        private static void DebugConfigForEasyTestBuild(string file){
            var text = File.ReadAllText(file);
            text = Regex.Replace(text, @"(EasyTest\|Any CPU\.ActiveCfg = )(Release)(\|Any CPU)", "$1Debug$3", RegexOptions.Singleline);
            File.WriteAllText(file, text);
        }

        private static IEnumerable<string> GetFiles(string rootDir,string pattern){
            var files = Directory.GetFiles(rootDir, pattern, SearchOption.AllDirectories);
            return files.Where(s => !_excludedDirs.Any(dir => s.IndexOf(dir, StringComparison.Ordinal) > -1));
        }

        private static void DeleteBackupFolders(string rootDir){
            var directories = Directory.EnumerateDirectories(rootDir,"Backup*",SearchOption.AllDirectories).ToList();
            foreach (var directory in directories){
                Directory.Delete(directory,true);
            }
        }

        static string GetVersion(string rootDir) {
            using (var fileStream = File.OpenRead(Path.Combine(rootDir, @"Xpand\Xpand.Utils\Properties\XpandAssemblyInfo.cs"))) {
                using (var streamReader = new StreamReader(fileStream)) {
                    return Regex.Match(streamReader.ReadToEnd(), "Version = \"(?<version>[^\"]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase)
                                .Groups["version"].Value;
                }
            }
        }
    }

}
