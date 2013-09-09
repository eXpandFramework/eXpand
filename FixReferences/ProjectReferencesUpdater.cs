using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FixReferences {
    class ProjectReferencesUpdater : Updater {
        readonly XNamespace _xNamespace = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

        readonly string[] _copyLocalReferences = new[]{
            "Xpand.ExpressApp.FilterDataStore", "Xpand.ExpressApp.FilterDataStore.Win",
            "Xpand.ExpressApp.FilterDataStore.Web", "Xpand.ExpressApp.ModelAdaptor", "Xpand.Persistent.BaseImpl"
        };

        readonly Dictionary<string, string> _requiredApplicationProjectReferences =
            new Dictionary<string, string>{
                {"Xpand.ExpressApp", "Xpand.Persistent.BaseImpl"},
                {"Xpand.ExpressApp.ExceptionHandling", "Xpand.Persistent.BaseImpl"},
                {"Xpand.ExpressApp.IO", "Xpand.Persistent.BaseImpl"},
                {"Xpand.Persistent.BaseImpl.JobScheduler", "Xpand.Persistent.BaseImpl"},
                {"Xpand.ExpressApp.WorldCreator", "Xpand.Persistent.BaseImpl"},
                {"Xpand.ExpressApp.PivotChart.Win", "Xpand.Persistent.BaseImpl"}
            };

        public ProjectReferencesUpdater(IDocumentHelper documentHelper, string rootDir) : base(documentHelper, rootDir) {
            
        }

        public override void Update(string file) {
            var document = DocumentHelper.GetXDocument(file);
            var directoryName = Path.GetDirectoryName(file) + "";
            if (IsApplicationProject(document)) {
                AddRequiredReferences(document, file);
            }
            UpdateReferences(document, directoryName, file);
            UpdateConfig(file);
            var licElement = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "EmbeddedResource" && element.Attribute("Include").Value == @"Properties\licenses.licx");
            if (licElement != null) {
                licElement.Remove();
                DocumentHelper.Save(document,file);
            }
            var combine = Path.Combine(Path.GetDirectoryName(file)+"", @"Properties\licenses.licx");
            if  (File.Exists(combine))
                File.Delete(combine);
        }

        void UpdateConfig(string file) {
            var config = Path.Combine(Path.GetDirectoryName(file) + "", "app.config");
            if (File.Exists(config)) {
                ReplaceToken(config);
            }
            else {
                config = Path.Combine(Path.GetDirectoryName(file) + "", "web.config");
                if (File.Exists(config)) {
                    ReplaceToken(config);
                }
            }
        }

        void ReplaceToken(string config) {     
            string readToEnd;
            using (var streamReader = new StreamReader(config)) {
                readToEnd = streamReader.ReadToEnd().Replace("c52ffed5d5ff0958", "b88d1754d700e49a");
            }
            using (var streamWriter = new StreamWriter(config)) {
                streamWriter.Write(readToEnd);
            }
        }

        void AddRequiredReferences(XDocument document, string file) {
            var referencesItemGroup = document.Descendants().First(element => element.Name.LocalName == "Reference").Parent;
            if (referencesItemGroup == null) throw new NullReferenceException("referencesItemGroup");

            foreach (string reference in RequiredReferencesThatDoNotExist(document)) {
                var referenceElement = new XElement(_xNamespace + "Reference");
                referenceElement.Add(new XAttribute("Include", reference));
                referencesItemGroup.Add(referenceElement);
                DocumentHelper.Save(document, file);
            }
        }
        IEnumerable<string> RequiredReferencesThatDoNotExist(XDocument document) {
            return _requiredApplicationProjectReferences.Where(reference => !AlreadyReferenced(document, reference.Value) &&
                HasReferenceRequirement(document, reference.Key)).Select(reference => reference.Value);
        }

        bool AlreadyReferenced(XDocument document, string reference) {
            return document.Descendants().Any(element => element.Name.LocalName == "Reference" && element.Attribute("Include").Value == reference);
        }

        bool HasReferenceRequirement(XDocument document, string reference) {
            return HasReferenceRequirementInProject(document, reference) || HasReferenceRequirementInReferenceProjects(document, reference);
        }
        bool HasReferenceRequirementInReferenceProjects(XDocument document, string reference) {
            var documents = document.Descendants().Where(element => element.Name.LocalName == "ProjectReference").Select(element
                => DocumentHelper.GetXDocument(Path.GetFullPath(element.Attribute("Include").Value)));
            return documents.Any(xDocument => HasReferenceRequirementInProject(xDocument, reference));
        }

        bool HasReferenceRequirementInProject(XDocument document, string reference) {
            return document.Descendants().Any(element => element.Name.LocalName == "Reference" && element.Attribute("Include").Value == reference);
        }

        bool IsApplicationProject(XDocument document) {
            var outputType = document.Descendants().First(element => element.Name.LocalName == "OutputType");
            return outputType != null && ((outputType.Value == "WinExe" || outputType.Value == "Exe") || (outputType.Value == "Library" && CheckWebGuid(document)));
        }

        bool CheckWebGuid(XDocument document) {
            var projectTypeGuids = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "ProjectTypeGuids");
            return projectTypeGuids != null && projectTypeGuids.Value.Split(';').Any(s => s.Contains("349c5851-65df-11da-9384-00065b846f21"));
        }

        void UpdateReferences(XDocument document, string directoryName, string file) {
            var references = document.Descendants().Where(IsXpandOrDXElement);
            foreach (XElement reference in references) {
                var attribute = reference.Attribute("Include");

                var value = Regex.Match(attribute.Value, "(Xpand.[^,]*)|(DevExpress.[^,]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Value;
                if (string.CompareOrdinal(attribute.Value, value) != 0) {
                    attribute.Value = value;
                    DocumentHelper.Save(document, file);
                }

                UpdateElementValue(reference, "SpecificVersion", "False", file, document);

                if (_copyLocalReferences.Contains(attribute.Value))
                    UpdateElementValue(reference, "Private", "True", file, document);

                if (reference.Attribute("Include").Value.StartsWith("Xpand.")) {
                    var path = CalcPathToXpandDll(directoryName) + attribute.Value + ".dll";
                    UpdateElementValue(reference, "HintPath", path, file, document);
                }
            }
        }
        void UpdateElementValue(XElement reference, string name, string value, string file, XDocument document) {
            var element = reference.Nodes().OfType<XElement>().FirstOrDefault(xelement => xelement.Name.LocalName == name);
            if (element == null) {
                element = new XElement(_xNamespace + name);
                reference.Add(element);
                element.Value = value;
                DocumentHelper.Save(document, file);
            } else if (string.CompareOrdinal(value, element.Value) != 0) {
                element.Value = value;
                DocumentHelper.Save(document, file);
            }
        }
        bool IsXpandOrDXElement(XElement element) {
            return element.Name.LocalName == "Reference" && Regex.IsMatch(element.Attribute("Include").Value, "(Xpand)|(DevExpress)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        string CalcPathToXpandDll(string project) {
            string path = null;
            while (project != RootDir) {
                path += @"..\";
                project = project.Substring(0, project.LastIndexOf(@"\", StringComparison.Ordinal));
            }
            return path + @"Xpand.DLL\";
        }
    }

}