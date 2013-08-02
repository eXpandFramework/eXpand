using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FixReferences {
    class Program {
        static readonly Dictionary<string, string> _requiredApplicationProjectReferences =
            new Dictionary<string, string>{
                {"Xpand.ExpressApp", "Xpand.Persistent.BaseImpl"},
                {"Xpand.ExpressApp.ExceptionHandling", "Xpand.Persistent.BaseImpl"},
                {"Xpand.ExpressApp.IO", "Xpand.Persistent.BaseImpl"},
                {"Xpand.Persistent.BaseImpl.JobScheduler", "Xpand.Persistent.BaseImpl"},
                {"Xpand.ExpressApp.WorldCreator", "Xpand.Persistent.BaseImpl"},
                {"Xpand.ExpressApp.PivotChart.Win", "Xpand.Persistent.BaseImpl"}
            };
        static readonly XNamespace _xNamespace = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

        static readonly string[] _copyLocalReferences = new[]
        {"Xpand.ExpressApp.FilterDataStore", "Xpand.ExpressApp.ModelAdaptor", "Xpand.Persistent.BaseImpl"};
        static string _rootDir;

        static void Main() {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            _rootDir = Path.GetFullPath(@"..\..\..");
            Execute();
        }

        public static bool Execute() {
            var version = GetVersion();
            Console.WriteLine(version);
            
            var files = Directory.GetFiles(_rootDir, "*.csproj", SearchOption.AllDirectories);
            foreach (var file in files) {
                var directoryName = Path.GetDirectoryName(file) + "";
                var document = GetXDocument(file);
                if (IsApplicationProject(document) ) {
                    AddRequiredReferences(document,file);
                }
                UpdateReferences(document, directoryName,file);
                
            }
            return true;
        }

        static void Save(XDocument document, string file) {
            document.Save(file, SaveOptions.None);
        }

        static void AddRequiredReferences(XDocument document, string file) {
            var referencesItemGroup = document.Descendants().First(element => element.Name.LocalName == "Reference").Parent;
            if (referencesItemGroup == null) throw new NullReferenceException("referencesItemGroup");

            foreach (string reference in RequiredReferencesThatDoNotExist(document)) {
                var referenceElement = new XElement(_xNamespace + "Reference");
                referenceElement.Add(new XAttribute("Include", reference));
                referencesItemGroup.Add(referenceElement);
                Save(document, file);
            }
        }

        static IEnumerable<string> RequiredReferencesThatDoNotExist(XDocument document) {
            return _requiredApplicationProjectReferences.Where(reference =>!AlreadyReferenced(document,reference.Value)&& 
                HasReferenceRequirement(document, reference.Key)).Select(reference => reference.Value);
        }

        static bool AlreadyReferenced(XDocument document, string reference) {
            return document.Descendants().Any(element => element.Name.LocalName == "Reference" && element.Attribute("Include").Value == reference);
        }

        static bool HasReferenceRequirement(XDocument document, string reference) {
            return HasReferenceRequirementInProject(document, reference)||HasReferenceRequirementInReferenceProjects(document,reference);
        }

        static bool HasReferenceRequirementInReferenceProjects(XDocument document, string reference) {
            var documents = document.Descendants().Where(element => element.Name.LocalName == "ProjectReference").Select(element 
                => GetXDocument(Path.GetFullPath(element.Attribute("Include").Value)));
            return documents.Any(xDocument => HasReferenceRequirementInProject(xDocument, reference));
        }

        static bool HasReferenceRequirementInProject(XDocument document, string reference) {
            return document.Descendants().Any(element =>element.Name.LocalName == "Reference" && element.Attribute("Include").Value == reference);
        }

        static bool IsApplicationProject(XDocument document) {
            var outputType = document.Descendants().First(element => element.Name.LocalName == "OutputType");
            return outputType != null && ((outputType.Value == "WinExe" || outputType.Value == "Exe") || (outputType.Value == "Library" && CheckWebGuid(document)));
        }

        static bool CheckWebGuid(XDocument document) {
            var projectTypeGuids = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "ProjectTypeGuids");
            return projectTypeGuids != null && projectTypeGuids.Value.Split(';').Any(s => s.Contains("349c5851-65df-11da-9384-00065b846f21"));
        }

        static void UpdateReferences(XDocument document, string directoryName, string file) {
            var references = document.Descendants().Where(IsXpandOrDXElement);
            foreach (XElement reference in references) {
                var attribute = reference.Attribute("Include");

                var value = Regex.Match(attribute.Value, "(Xpand.[^,]*)|(DevExpress.[^,]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Value;
                if (string.CompareOrdinal(attribute.Value, value) != 0) {
                    attribute.Value =value;
                    Save(document, file);
                }

                UpdateElementValue(reference, "SpecificVersion", "False",file,document);

                if (_copyLocalReferences.Contains(attribute.Value))
                    UpdateElementValue(reference, "Private","True", file, document);

                if (reference.Attribute("Include").Value.StartsWith("Xpand.")) {
                    var path = CalcPathToXpandDll(directoryName) + attribute.Value + ".dll";
                    UpdateElementValue(reference, "HintPath", path, file, document);
                }
            }
        }

        static void UpdateElementValue(XElement reference, string name, string value, string file, XDocument document) {
            var element = reference.Nodes().OfType<XElement>().FirstOrDefault(xelement => xelement.Name.LocalName == name);
            if (element == null) {
                element = new XElement(_xNamespace + name);
                reference.Add(element);
                element.Value = value;
                Save(document, file);
            }
            else if (string.CompareOrdinal(value,element.Value)!=0) {
                element.Value = value;
                Save(document, file);
            }
        }

        static bool IsXpandOrDXElement(XElement element) {
            return element.Name.LocalName == "Reference" && Regex.IsMatch(element.Attribute("Include").Value, "(Xpand)|(DevExpress)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        static string CalcPathToXpandDll(string project) {
            string path = null;
            while (project != _rootDir) {
                path += @"..\";
                project = project.Substring(0, project.LastIndexOf(@"\", StringComparison.Ordinal));
            }
            return path + @"Xpand.DLL\";
        }

        static XDocument GetXDocument(string file) {
            Environment.CurrentDirectory = Path.GetDirectoryName(file)+"";
            XDocument document;
            using (var fileStream = File.OpenRead(file)) {
                document = XDocument.Load(fileStream);
            }
            return document;
        }

        static string GetVersion() {
            using (var fileStream = File.OpenRead(Path.Combine(_rootDir, @"Xpand\Xpand.Utils\Properties\AssemblyInfo.cs"))) {
                using (var streamReader = new StreamReader(fileStream)) {
                    return Regex.Match(streamReader.ReadToEnd(), "Version = \"(?<version>[^\"]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase)
                                .Groups["version"].Value;
                }
            }
        }
    }
}
