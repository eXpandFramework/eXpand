using System;
using System.IO;

namespace FixReferences {
    class Program {

        static void Main() {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            Execute(Path.GetFullPath(@"..\..\.."));
        }

        public static bool Execute(string rootDir) {
            var files = Directory.GetFiles(rootDir, "*.csproj", SearchOption.AllDirectories);
            var documentHelper = new DocumentHelper();
            foreach (var file in files) {
                var projectReferencesUpdater = new ProjectReferencesUpdater(documentHelper,rootDir);
                projectReferencesUpdater.Update(file);
                var nugetUpdater = new NugetUpdater(documentHelper, rootDir);
                nugetUpdater.Update(file);
            }
            var xpandBuildUpdater = new XpandMSBuildUpdater(documentHelper, rootDir);
            xpandBuildUpdater.Update(Path.Combine(rootDir, "Xpand.Build"));
            return true;
        }
    }
}
