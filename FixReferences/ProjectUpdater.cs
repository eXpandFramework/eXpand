using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FixReferences {
    class ProjectUpdater : Updater {
        private readonly string _version;
        readonly XNamespace _xNamespace = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

        readonly string[] _copyLocalReferences ={
            "Xpand.ExpressApp.FilterDataStore", "Xpand.ExpressApp.FilterDataStore.Win",
            "Xpand.ExpressApp.FilterDataStore.Web", "Xpand.ExpressApp.ModelAdaptor", "Xpand.Persistent.BaseImpl","DevExpress.Web.ASPxThemes."
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

        public ProjectUpdater(IDocumentHelper documentHelper, string rootDir,string version) : base(documentHelper, rootDir){
            _version = version;
        }

        public override void Update(string file) {
            var document = DocumentHelper.GetXDocument(file);
            var directoryName = Path.GetDirectoryName(file) + "";
            if (IsApplicationProject(document)) {
                AddRequiredReferences(document, file);
            }
            UpdateReferences(document, directoryName, file);
            UpdateConfig(file);
            if (SyncConfigurations(document))
                DocumentHelper.Save(document, file);

            var licElement = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "EmbeddedResource" && element.Attribute("Include").Value == @"Properties\licenses.licx");
            if (licElement != null) {
                licElement.Remove();
                DocumentHelper.Save(document,file);
            }
            var combine = Path.Combine(Path.GetDirectoryName(file)+"", @"Properties\licenses.licx");
            if  (File.Exists(combine))
                File.Delete(combine);

            UpdateVs2010Compatibility(document, file);
        }

        private bool SyncConfigurations(XDocument document){
            var debugOutputPath = GetOutputPath(document,"Debug").Value;
            if (debugOutputPath.ToLower().TrimEnd('\\').EndsWith("xpand.dll")){
                var releaseOutputPath = GetOutputPath(document,"Release");
                if (releaseOutputPath.Value+"" != debugOutputPath){
                    releaseOutputPath.Value = debugOutputPath;
                    return true;
                }
            }
            return false;
        }

        private XElement GetOutputPath(XDocument document, string configuration){
            return document.Descendants().Where(element => element.Name.LocalName=="OutputPath").First(element =>{
                if (element.Parent != null){
                    var attribute = element.Parent.Attribute("Condition");
                    if (attribute != null){
                        var value = attribute.Value+"";
                        return new[]{"Configuration",configuration}.All(value.Contains);
                    }
                }
                return false;
            });
        }

        void UpdateVs2010Compatibility(XDocument document, string file) {
            var elements = document.Descendants().Where(element 
                => element.Name.LocalName == "VSToolsPath" || element.Name.LocalName == "VisualStudioVersion" );
            var xElements = elements as XElement[] ?? elements.ToArray();
            bool save=xElements.Any();
            xElements.Remove();


            elements=document.Descendants().Where(element 
                => element.Name.LocalName == "Import" &&
                (element.Attribute("Project").Value.StartsWith("$(MSBuildExtensionsPath)")||
                element.Attribute("Project").Value.StartsWith("$(MSBuildExtensionsPath32)")));
            var enumerable = elements as XElement[] ?? elements.ToArray();
            if (!save)
                save = enumerable.Any();

            if (IsWeb(document, GetOutputType(document))&& !document.Descendants().Any(element =>
                element.Name.LocalName == "Import" && element.Attribute("Project").Value.StartsWith("$(VSToolsPath)")&&
                element.Attribute("Condition").Value.StartsWith("'$(VSToolsPath)' != ''"))) {
                var element = new XElement(_xNamespace+"Import");
                element.SetAttributeValue("Project",@"$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets");
                element.SetAttributeValue("Condition", @"'$(VSToolsPath)' != ''");
                Debug.Assert(document.Root != null, "document.Root != null");
                document.Root.Add(element);
                save = true;
            }
            
            enumerable.Remove();

            if (save)
                DocumentHelper.Save(document, file);
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
            var functionalTestsPath = Path.GetDirectoryName(file)+@"\FunctionalTests";
            if (Directory.Exists(functionalTestsPath)){
                config = Path.Combine(functionalTestsPath,"Config.xml");
                if (File.Exists(config)){
                    ReplaceToken(config);
                    UpdateAdapterVersion(config);
                }
            }
        }

        private void UpdateAdapterVersion(string config){
            string readToEnd;
            using (var streamReader = new StreamReader(config)){
                var toEnd = streamReader.ReadToEnd();
                readToEnd = Regex.Replace(toEnd, @"(<Alias Name=""(Win|Web)AdapterAssemblyName"" Value=""Xpand\.ExpressApp\.EasyTest[^=]*=)([.\d]*)", "${1}" + _version);
                readToEnd = Regex.Replace(readToEnd, @"((<Alias Name=""(Win|Web)AdapterAssemblyName"" Value=""Xpand\.ExpressApp\.EasyTest[^=]*=).*?, PublicKeyToken=)(b88d1754d700e49a)", "$1c52ffed5d5ff0958", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            using (var streamWriter = new StreamWriter(config)) {
                streamWriter.Write(readToEnd);
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

        bool IsApplicationProject(XDocument document){
            var outputType = GetOutputType(document);
            return outputType != null && (IsExe(outputType) || (IsWeb(document, outputType)));
        }

        private bool IsWeb(XDocument document, XElement outputType){
            return outputType.Value == "Library" && CheckWebGuid(document);
        }

        private static bool IsExe(XElement outputType){
            return (outputType.Value == "WinExe" || outputType.Value == "Exe");
        }

        private static XElement GetOutputType(XDocument document){
            var outputType = document.Descendants().First(element => element.Name.LocalName == "OutputType");
            return outputType;
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

                if (_copyLocalReferences.Any(s => attribute.Value.StartsWith(s)))
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