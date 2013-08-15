using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace FixReferences {
    class NugetUpdater:Updater {
        readonly XNamespace _xNamespace = XNamespace.Get("http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd");

        public NugetUpdater(IDocumentHelper documentHelper,string rootDir) : base(documentHelper, rootDir) {

        }

        public override void Update(string file) {
            var document = DocumentHelper.GetXDocument(file);
            var nugetPkgElement = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "NugetPkg");
            if (nugetPkgElement != null) {
                var fileInfo = GetNugetSpecInfo(nugetPkgElement.Value);
                var files = fileInfo.Item1;
                files.RemoveNodes();
                var xpandReferences = document.Descendants().Where(element => element.Name.LocalName == "Reference").Select(element
                    => element.Attribute("Include").Value).Where(s => s.StartsWith("Xpand"));
                foreach (var xpandReference in xpandReferences) {
                    var xElement = new XElement(_xNamespace + "file");
                    xElement.Add(new XAttribute("src", @"\Build\Temp\" + xpandReference + ".dll"));
                    xElement.Add(new XAttribute("target", @"lib\" + xpandReference + ".dll"));
                    files.Add(xElement);
                }
                DocumentHelper.Save(files.Document, fileInfo.Item2);
            }
        }
        Tuple<XElement, string> GetNugetSpecInfo(string value) {
            foreach (var file in Directory.GetFiles(Path.Combine(RootDir, @"Resource\Nuget"), "*.nuspec")) {
                var xDocument = DocumentHelper.GetXDocument(file);
                if (xDocument.Descendants().Any(element => element.Name.LocalName.ToLower() == "id" && element.Value == value)) {
                    var xElement = xDocument.Descendants().First(element => element.Name.LocalName == "files");
                    return new Tuple<XElement, string>(xElement, file);
                }
            }
            throw new FileNotFoundException(value);
        }

    }

}
