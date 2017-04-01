using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using EnvDTE80;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Options{
    public class OptionClass {
        private static readonly DTE2 _dte = DteExtensions.DTE;
        private static readonly string _path;

        static OptionClass() {
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Xpand.VSIX.Options.xml");
            Instance = GetOptionClass();
        }

        public static OptionClass Instance { get; }

        public bool KillModelEditor { get; set; }


        public bool DebugME { get; set; }


        public bool SpecificVersion { get; set; }


        public string Token { get; set; }

        public string TestExecutorPath { get; set; }


        public string ProjectConverterPath { get; set; }

        private static OptionClass GetOptionClass() {
            var optionClass = new OptionClass();
            try {
                var xmlSerializer = new XmlSerializer(typeof(OptionClass));
                using (var stream = File.Open(_path, FileMode.OpenOrCreate)){
                    using (var streamReader = new StreamReader(stream)){
                        var allText = streamReader.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(allText)){
                            var xmlReader = XmlReader.Create(new StringReader(allText));
                            if (xmlSerializer.CanDeserialize(xmlReader))
                                optionClass = (OptionClass) xmlSerializer.Deserialize(xmlReader);
                        }
                    }
                }
                
            }
            catch (Exception e) {
                _dte.LogError(e.ToString());
                _dte.WriteToOutput(e.ToString());
            }
            return optionClass;

        }

        [XmlArray]
        public BindingList<ConnectionString> ConnectionStrings { get; } = new BindingList<ConnectionString>();

        [XmlArray]
        public BindingList<ReferencedAssembliesFolder> ReferencedAssembliesFolders { get; } = new BindingList<ReferencedAssembliesFolder>();

        [XmlArray]
        public BindingList<ExceptionsBreak> Exceptions { get; } = new BindingList<ExceptionsBreak>();

        [XmlArray]
        public BindingList<ME> MEs { get; } = new BindingList<ME>();

        [XmlArray]
        public BindingList<SourceCodeInfo> SourceCodeInfos { get; } = new BindingList<SourceCodeInfo>();

        public bool DisableExceptions { get; set; }

        public string DefaultConfiguration => "Debug";

        public void Save() {
            var stringBuilder = new StringBuilder();
            new XmlSerializer(typeof(OptionClass)).Serialize(XmlWriter.Create(stringBuilder), Instance);
            File.WriteAllText(_path, stringBuilder.ToString());
        }
    }
    public class ConnectionString : OptionClassBase {
        public string Name { get; set; }
    }

    public class ReferencedAssembliesFolder : OptionClassBase {

        public string Folder { get; set; }
    }
    public class ExceptionsBreak : OptionClassBase {

        public string Exception { get; set; }

        public bool Break { get; set; }
    }

    public class ME : OptionClassBase {
        public string Path { get; set; }
    }


    public class SourceCodeInfo : OptionClassBase {
        public string RootPath { get; set; }
        public string ProjectRegex { get; set; }

        public int Count => ProjectPaths.Count;

        [XmlArray]
        public List<ProjectInfo> ProjectPaths { get; } = new List<ProjectInfo>();

        public override string ToString() {
            return "RootPath:" + RootPath + " ProjectRegex=" + ProjectRegex + " Count=" + Count;
        }
    }

    public class ProjectInfo {
        public string Path { get; set; }
        public string OutputPath { get; set; }
    }

    public class OptionClassBase {
    }
}
