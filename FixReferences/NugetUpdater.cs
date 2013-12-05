using System.Linq;
using System.Xml.Linq;

namespace FixReferences {
    class NugetUpdater:Updater {
        readonly string _version;
        internal readonly XNamespace _xNamespace = XNamespace.Get("http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd");

        public NugetUpdater(IDocumentHelper documentHelper, string rootDir, string version) : base(documentHelper, rootDir) {
            _version = version;
        }

        public override void Update(string file) {
            var document = DocumentHelper.GetXDocument(file);
            var versionElement = document.Descendants().First(element => element.Name.LocalName == "version");
            versionElement.Value = _version;
            var dependenciesElement = document.Descendants().FirstOrDefault(element => element.Name.LocalName.ToLower() == "dependencies");
            if (dependenciesElement != null)
                foreach (var element in dependenciesElement.Elements()) {
                    element.SetAttributeValue("version",_version);
                }
            DocumentHelper.Save(document, file);
        }
    }

}
