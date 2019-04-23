using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MoreLinq;

namespace BuildHelper {
    class NuspecUpdater:Updater {
        readonly string _version;
        private readonly string[] _projects;
        private readonly string[] _nuspecs;
        internal static readonly XNamespace XNamespace = XNamespace.Get("http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd");
        
        public NuspecUpdater(IDocumentHelper documentHelper, string rootDir, string version, string[] projects, string[] nuspecs)
            : base(documentHelper, rootDir){
            
            _version = version;
            _projects = projects;
            _nuspecs = nuspecs;
        }

        private static IEnumerable<string> GetProjects(string file,string[] projects) {
            if ($"{Path.GetFileName(file)}".StartsWith("All_"))
                return Enumerable.Empty<string>();
            var nuspecFileNames = GetNuspecFiles(file);
            projects = projects.Where(s => nuspecFileNames.Contains(AdjustName((Path.GetFileNameWithoutExtension(s))))).ToArray();
            if (!projects.Any())
                throw new NotImplementedException(file);
            return projects;
        }

        private static HashSet<string> GetNuspecFiles(string file){
            var hashSet = new HashSet<string>();
            var nuspecFileName = (Path.GetFileNameWithoutExtension(file) + "").ToLowerInvariant();
            if (nuspecFileName.StartsWith("system"))
                hashSet.Add(nuspecFileName.Replace("system.", "").Replace("system", ""));
            else if (nuspecFileName == "lib"){
                hashSet = GetLibHashSet();
            }
            else{
                hashSet.Add(nuspecFileName);
            }
            return hashSet;
        }

        private static HashSet<string> GetLibHashSet(){
            return new HashSet<string>{"utils", "xpo", "persistent.base", "persistent.baseimpl"};
        }

        private static string AdjustName(string name){
            return name.Replace("Xpand.ExpressApp.", "").Replace("Xpand.ExpressApp", "").Replace("Xpand.", "").ToLowerInvariant();
        }

        public override void Update(string file) {
            var document = DocumentHelper.GetXDocument(file);
            var projects = GetProjects(file,_projects).ToArray();
            var isLibSpec = (Path.GetFileNameWithoutExtension(file)+"").ToLowerInvariant() == "lib";
            if (isLibSpec){
                projects = projects.Where(s => !s.Contains("BaseImpl")).ToArray();
            }
            
            UpdateDependencies(document,projects, isLibSpec);
            UpdateFiles(document, projects, isLibSpec);
            if (isLibSpec){
                var filesElement = document.Descendants(XNamespace + "files").First();
                filesElement.Add(NewFileElement("Xpand.Persistent.BaseImpl"));
                filesElement.Add(NewFileElement("Xpand.Persistent.BaseImpl", ".pdb"));
            }
            DocumentHelper.Save(document, file);
        }

        private void UpdateFiles(XDocument document, IEnumerable<string> projects, bool isLibSpec){
            var allReferences = GetReferences(projects, "System").ToArray();
            var dependencies = GetDependencies(allReferences, isLibSpec).Select(pair => pair.Key).ToArray();
            var filesElement = document.Descendants(XNamespace + "files").First();
            var descendantNodes = filesElement.DescendantNodes().ToArray();
            for (int index = descendantNodes.ToArray().Length - 1; index >= 0; index--) {
                var descendantNode = descendantNodes.ToArray()[index];
                descendantNode.Remove();
            }
            CreateMainFiles(allReferences, filesElement);
            CreateReferenceFiles(allReferences, dependencies, filesElement);
        }

        private void CreateReferenceFiles(IEnumerable<KeyValuePair<string, IEnumerable<XElement>>> allReferences, IEnumerable<XElement> dependencies, XElement filesElement){
            var xElements = allReferences.SelectMany(pair => pair.Value).DistinctBy(element => element.Attribute("Include")?.Value);
            var elements = xElements.Except(dependencies).Except(filesElement.Elements());
            foreach (var assemblyName in elements.Select(GetAssemblyName)){
                var newFileElement = NewFileElement(assemblyName);
                if (filesElement.DescendantNodes().Cast<XElement>().All(element => element.Attribute("src")?.Value != newFileElement.Attribute("src")?.Value)) {
                    filesElement.Add(newFileElement);
                }
            }
        }

        private void CreateMainFiles(IEnumerable<KeyValuePair<string, IEnumerable<XElement>>> allReferences, XElement filesElement){
            var keyValuePairs = allReferences.OrderBy(pair => Path.GetFileNameWithoutExtension(pair.Key)).DistinctBy(pair => pair.Key);
            foreach (var xDocument in keyValuePairs.Select(pair => XDocument.Load(pair.Key))){
                var assemblyName = xDocument.Descendants(ProjectUpdater.XNamespace + "AssemblyName").First().Value;
                filesElement.Add(NewFileElement(assemblyName));
                filesElement.Add(NewFileElement(assemblyName, ".pdb"));
            }
        }

        private XElement NewFileElement(string assemblyName,string extension=".dll"){
            var content = new XElement(XNamespace + "file");
            content.SetAttributeValue("src", @"\Build\Temp\" + assemblyName + extension);
            content.SetAttributeValue("target", @"lib\net40\" + assemblyName + extension);
            return content;
        }

        private void UpdateDependencies(XDocument document, IEnumerable<string> projects, bool isLibSpec){
            var allReferences = GetReferences(projects).ToArray();
            var dependencies = GetDependencies(allReferences, isLibSpec).ToArray();
            var metadataElement = document.Descendants(XNamespace+"metadata").First();
            metadataElement.Descendants(XNamespace+"dependencies").Remove();
            var dependenciesElement = new XElement(XNamespace + "dependencies");
            var packages = dependencies.Select(pair 
                => new { Name = GetAssemblyName(pair.Key), pair.Value.Version ,pair.Value.Id}).Select(arg 
                    => new { Id = GetPackageId(arg.Name,arg.Id), arg.Version }).OrderBy(arg => arg.Id).GroupBy(arg => arg.Id).Select(grouping => grouping.First()).ToArray();
            foreach (var package in packages) {
                var dependencyElement = new XElement(XNamespace + "dependency");
                dependencyElement.SetAttributeValue("id",package.Id);
                dependencyElement.SetAttributeValue("version",package.Version);
                dependenciesElement.Add(dependencyElement);
            }
            metadataElement.Add(dependenciesElement);
        }
        
        private string GetPackageId(string assemblyName, string id){
            if (assemblyName.StartsWith("Xpand")&&!assemblyName.StartsWith("Xpand.XAF.Modules")){
                var adjustName = AdjustName(assemblyName).ToLowerInvariant();
                var nuspec = FindNuspec(adjustName);
                if (nuspec == null){
                    var strings = new []{"utils","xpo","persistent.base","persistent.baseimpl"};
                    if (strings.Any(adjustName.Contains))
                        nuspec = FindNuspec("lib");
                    else if (adjustName.Contains("win"))
                        nuspec = FindNuspec("system.win");
                    else if (adjustName.Contains("web"))
                        nuspec = FindNuspec("system.web");
                    else if (adjustName == "")
                        nuspec = FindNuspec("system");
                }
                return XDocument.Load(nuspec ?? throw new InvalidOperationException()).Descendants(XNamespace + "id").First().Value;
            }
            return id;
        }

        private string FindNuspec(string adjustName){
            return _nuspecs.FirstOrDefault(s => adjustName == (Path.GetFileNameWithoutExtension(s)+"").ToLowerInvariant());
        }

        private IEnumerable<KeyValuePair<XElement, (string Id,string Version)>> GetDependencies(IEnumerable<KeyValuePair<string, IEnumerable<XElement>>> references, bool isLibSpec) {
            var elements = new List<KeyValuePair<XElement, (string, string)>>();
            foreach (var reference in references){
                var path = Path.Combine(Path.GetDirectoryName(reference.Key)+"", "packages.config");
                foreach (var element in reference.Value){
                    var assemblyName = GetAssemblyName(element).ToLowerInvariant();
                    var version = _version;
                    string packageId = null;
                    if(!assemblyName.StartsWith("xpand")||assemblyName.StartsWith("xpand.xaf.modules")) {
                        packageId = GetPackageId(element)??assemblyName;
                        var packagesConfig = (File.Exists(path) ? File.ReadAllText(path) : "").ToLowerInvariant();
                        var regex = new Regex("<package id=\"" + packageId + "\" .*version=\"([^\"]*).*/>",RegexOptions.IgnoreCase);
                        var match = regex.Match(packagesConfig);
                        if (match.Success){
                            version = match.Groups[1].Value;
                        }
                        else{
                            continue;
                        }
                    }
                    if (isLibSpec && GetLibHashSet().Any(s => assemblyName.ToLowerInvariant().Contains(s)))
                        continue;
                    elements.Add(new KeyValuePair<XElement, (string,string)>(element, (packageId,version)));
                }
            }
            return elements.DistinctBy(pair => pair.Key.Attribute("Include")?.Value);
        }

        private string GetPackageId(XElement element){
            var xElement = element.Descendants(ProjectUpdater.XNamespace+"HintPath").FirstOrDefault();
            if (xElement!=null){
                var regex = new Regex(@"Support\\_third_party_assemblies\\Packages\\([^\\]*)",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var match = regex.Match(xElement.Value);
                if (match.Success){
                    var value = match.Groups[1].Value;
                    var index = Regex.Match(value, @"(\.([\d]+))", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Index;
                    return value.Substring(0, index);
                }
            }
            return null;
        }

        private string GetAssemblyName(XElement element){
            var value = element.Attribute("Include")?.Value;
            if (value != null){
                var indexOf = value.IndexOf(",", StringComparison.Ordinal);
                return indexOf == -1 ? value : value.Substring(0, indexOf);
            }
            return null;
        }

        private IEnumerable<KeyValuePair<string,IEnumerable<XElement>>> GetReferences(IEnumerable<string> projects,params  string[] excluded){
            if (excluded==null)
                excluded=new string[0];
            return projects.Select(s =>{
                var document = XDocument.Load(s);
                var strings = new []{"DevExpress","Microsoft"}.Concat(excluded);
                var xElements = document.Descendants(ProjectUpdater.XNamespace + "Reference")
                    .Where(element => !strings.Any(s1 =>{
                        var attribute = element.Attribute("Include");
                        return attribute != null && attribute.Value.StartsWith(s1);
                    }));
                if (Program.Options.AfterBuild){
                    var assemblyPath = Path.Combine(RootDir, (@"Xpand.dll\" + Path.GetFileNameWithoutExtension(s) + ".dll"));
                    var assemblies = Assembly.ReflectionOnlyLoadFrom(assemblyPath).GetReferencedAssemblies().Select(name => name.Name).ToArray();
                    xElements = xElements.Where(element => {
                        var assemblyName = GetAssemblyName(element);
                        return assemblies.Contains(assemblyName)||assemblyName.ToUpper().Contains("POSIX");
                    });
                }
                return new KeyValuePair<string, IEnumerable<XElement>>(s, xElements);
            });
        }
    }

}
