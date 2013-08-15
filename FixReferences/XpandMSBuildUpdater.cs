using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace FixReferences {
    class XpandMSBuildUpdater:Updater {
        readonly XNamespace _xNamespace = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");
        public XpandMSBuildUpdater(IDocumentHelper documentHelper, string rootDir) : base(documentHelper, rootDir) {
        }

        public override void Update(string file) {
            var nuspecFiles = DocumentHelper.SavedFiles.Select(Path.GetFileName).Where(s => Path.GetExtension(s) == ".nuspec").ToList();
            var document = DocumentHelper.GetXDocument(file);
            CreateNugetPackageElements(document,nuspecFiles);
            var nuGetElement = document.Descendants().First(element => element.Name.LocalName == "Target" && element.Attribute("Name").Value == "NuGet");
            nuGetElement.RemoveNodes();
            foreach (var nuspecFile in nuspecFiles.Select(Path.GetFileNameWithoutExtension)) {
                var element = new XElement(_xNamespace+"Exec");
                element.Add(new XAttribute("ContinueOnError", "false"));
                element.Add(new XAttribute("Command",@"Resource\Tool\NuGet.exe push Build\NuGet\"+nuspecFile+
                                                             ".$(Version).nupkg $(NuGetApiKey)"));
                nuGetElement.Add(element);
            }
            DocumentHelper.Save(document, file);
        }

        void CreateNugetPackageElements(XDocument document, IEnumerable<string> nuspecFiles) {
            var nugetPackageElement =document.Descendants().First(
                element => element.Name.LocalName == "Target" && element.Attribute("Name").Value == "NuGetPackage");
            nugetPackageElement.RemoveNodes();
            AddMakeDirElement(nugetPackageElement);
            foreach (var nuspecFile in nuspecFiles) {
                var xElement = new XElement(_xNamespace+"Exec");
                xElement.Add(new XAttribute("ContinueOnError", "false"));
                xElement.Add(new XAttribute("Command", @"Resource\Tool\NuGet.exe pack Resource\NuGet\" + nuspecFile+
                                                       @" -BasePath $(MSBuildProjectDirectory) -OutputDirectory $(BuildPath)\NuGet -Version $(Version)"));
                nugetPackageElement.Add(xElement);
            }
        }

        void AddMakeDirElement(XElement nugetPackageElement) {
            var xElement = new XElement(_xNamespace+"MakeDir");
            xElement.Add(new XAttribute("Directories", @"$(BuildPath)\NuGet"));
            nugetPackageElement.Add(xElement);
        }
    }
}