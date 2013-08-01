using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FixReferences {
    class Program {
        static readonly Dictionary<string, string> _requiredApplicationProjectReferences = new Dictionary<string, string> { { "Xpand.Persistent.BaseImpl", "Xpand.ExpressApp" } };
        static readonly XNamespace _xNamespace = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");
        static readonly string[] _copyLocalReferences = new[] { "Xpand.ExpressApp.FilterDataStore", "Xpand.ExpressApp.ModelAdaptor", "Xpand.Persistent.BaseImpl" };
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
                    AddRequiredReferences(document);
                }
                UpdateReferences(document, directoryName);
                document.Save(file, SaveOptions.None);
            }
            return true;
        }

        static void AddRequiredReferences(XDocument document) {
            var referencesItemGroup = document.Descendants().First(element => element.Name.LocalName == "Reference").Parent;
            if (referencesItemGroup == null) throw new NullReferenceException("referencesItemGroup");
            foreach (string reference in RequiredReferencesThatDoNotExist(document)) {
                var referenceElement = new XElement(_xNamespace + "Reference");
                referenceElement.Add(new XAttribute("Include", reference));
                referencesItemGroup.Add(referenceElement);
            }
        }


        static IEnumerable<string> RequiredReferencesThatDoNotExist(XDocument document) {
            return _requiredApplicationProjectReferences.Where(reference =>!AlreadyReferenced(document,reference.Key)&& CheckReferenceRequirement(document, reference.Value))
                                                     .Select(reference => reference.Key);
        }

        static bool AlreadyReferenced(XDocument document, string reference) {
            return document.Descendants().Any(element => element.Name.LocalName == "Reference" && element.Attribute("Include").Value == reference);
        }

        static bool CheckReferenceRequirement(XDocument document, string reference) {
            return document.Descendants().Any(element =>element.Name.LocalName == "Reference" && element.Attribute("Include").Value == reference);
        }

        static bool IsApplicationProject(XDocument document) {
            var outputType = document.Descendants().First(element => element.Name.LocalName == "OutputType");
            return outputType != null && ((outputType.Value == "WinExe" || outputType.Value == "Exe") || (outputType.Value == "Library" && CheckWebGuid(document)));
        }

        static bool CheckWebGuid(XDocument document) {
            var projectTypeGuids = document.Descendants("ProjectTypeGuids").FirstOrDefault();
            return projectTypeGuids != null && projectTypeGuids.Value.Split(';').Any(s => s.Contains("349c5851-65df-11da-9384-00065b846f21"));
        }

        static void UpdateReferences(XDocument document,  string directoryName) {
            var references = document.Descendants().Where(element => element.Name.LocalName == "Reference" && Regex.IsMatch(element.Attribute("Include").Value, "(Xpand)|(DevExpress)", RegexOptions.Singleline | RegexOptions.IgnoreCase));
            foreach (XElement reference in references) {
                var attribute = reference.Attribute("Include");
                attribute.Value =
                    Regex.Match(attribute.Value, "(Xpand.[^,]*)|(DevExpress.[^,]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Value;
                reference.RemoveNodes();
                UpdateReferences(reference, attribute.Value, directoryName);
            }
        }
        static void UpdateReferences(XElement reference, string assemblyName, string directoryName) {

            reference.Add(new XElement(_xNamespace + "SpecificVersion") { Value = "False" });
            if (_copyLocalReferences.Contains(assemblyName))
                reference.Add(new XElement(_xNamespace + "Private") { Value = "True" });

            if (reference.Attribute("Include").Value.StartsWith("Xpand.")) {
                reference.Add(new XElement(_xNamespace + "HintPath") {
                    Value = CalcPathToXpandDll(directoryName) + assemblyName + ".dll"
                });
            }
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
