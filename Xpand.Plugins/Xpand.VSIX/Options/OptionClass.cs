using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using EnvDTE80;
using Microsoft.Win32;
using Xpand.VSIX.Commands;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Options{
    public class OptionClass {
        private static readonly DTE2 DTE = DteExtensions.DTE;
        public static readonly string Path;

        static OptionClass() {
            try{
                var directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Xpand\VSIX\";
                Path = System.IO.Path.Combine(directory, "Xpand.VSIX.Options.xml");
                if (File.Exists(Path))
                    Instance = GetOptionClass();
                else{
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    Instance = new OptionClass{
                        KillModelEditor = true,
                        SpecificVersion = true
                    };
                    
                    Instance.ConnectionStrings.Add(new ConnectionString() { Name = "ConnectionString" });
                    Instance.ConnectionStrings.Add(new ConnectionString() { Name = "WorldCreatorConnectionString" });
                    Instance.ConnectionStrings.Add(new ConnectionString() { Name = "EasyTestConnectionString" });
                    Instance.ConnectionStrings.Add(new ConnectionString() { Name = "NorthWind" });
                    var registryKey = Registry.LocalMachine.OpenSubKey(@"Software\WOW6432node\DevExpress\Components\");
                    if (registryKey != null)
                        foreach (var keyName in registryKey.GetSubKeyNames()) {
                            directory = (string)registryKey.OpenSubKey(keyName)?.GetValue("RootDirectory");
                            Instance.ReferencedAssembliesFolders.Add(new ReferencedAssembliesFolder() { Folder = directory });
                            var sourceCodeInfo = new SourceCodeInfo { ProjectRegex = "DevExpress.*csproj", RootPath = directory };
                            sourceCodeInfo.AddProjectPaths();
                            Instance.SourceCodeInfos.Add(sourceCodeInfo);
                        }
                    
                    Instance.Exceptions.Add(new ExceptionsBreak() { Break = false, Exception = typeof(FileNotFoundException).FullName });
                    Instance.Exceptions.Add(new ExceptionsBreak() { Break = false, Exception = typeof(SqlException).FullName });
                    Instance.DisableExceptions = false;
                    Instance.SourceCodeInfos.Add(new SourceCodeInfo { ProjectRegex = "Xpand.*csproj" });                
                }
                
                if (!Instance.DteCommandsBindings){
                    Instance.DteCommandsBindings = true;
                    Instance.DteCommands.Add(new DteCommand(){Command = nameof(BuildSelectionCommand),Shortcut = "Global::Ctrl+Alt+Enter"});
                    Instance.DteCommands.Add(new DteCommand(){Command = nameof(DuplicateLineCommand),Shortcut = "Text Editor::Ctrl+D"});
                    Instance.DteCommands.Add(new DteCommand(){Command = nameof(DropDataBaseCommand),Shortcut = "Global::Ctrl+Shift+Alt+D"});
                    Instance.DteCommands.Add(new DteCommand(){Command = $"Run{nameof(EasyTestCommand)}",Shortcut = "Text Editor::Alt+T"});
                    Instance.DteCommands.Add(new DteCommand(){Command = $"Debug{nameof(EasyTestCommand)}",Shortcut = "Text Editor::Alt+D"});
                    Instance.DteCommands.Add(new DteCommand(){Command = $"{nameof(FindInSolutionCommand)}",Shortcut = "Text Editor::Alt+Shift+L"});
                    Instance.DteCommands.Add(new DteCommand(){Command = $"{nameof(KillIISExpressCommand)}",Shortcut = "Global::Ctrl+Alt+Shift+I"});
                    Instance.DteCommands.Add(new DteCommand(){Command = $"{nameof(LoadProjectFromReferenceCommand)}",Shortcut = "Solution Explorer::Ctrl+Alt+Shift+L"});
                    Instance.DteCommands.Add(new DteCommand(){Command = $"{nameof(NavigateNextSubwordCommand)}",Shortcut = "Text Editor::ALT+Right Arrow"});
                    Instance.DteCommands.Add(new DteCommand(){Command = $"{nameof(NavigatePreviousSubwordCommand)}",Shortcut = "Text Editor::ALT+Left Arrow"});
                    Instance.DteCommands.Add(new DteCommand(){Command = $"{nameof(ShowModelsWindowCommand)}",Shortcut = "Global::Ctrl+Alt+Shift+M"});
                    Instance.DteCommands.Add(new DteCommand(){Command = $"{nameof(ShowOptionsCommand)}",Shortcut = "Global::Alt+Shift+0"});
                }

            }
            catch (Exception e){
                DTE.LogError(e.ToString());
                throw;
            }
        }

        public bool DteCommandsBindings{ get; set; }

        public static OptionClass Instance { get; }

        public bool KillModelEditor { get; set; }


        public bool ShowErrorsInMessageBox { get; set; }
        public bool DebugME { get; set; }


        public bool SpecificVersion { get; set; }


        public string Token { get; set; }

        public string TestExecutorPath { get; set; }


        public string ProjectConverterPath { get; set; }

        private static OptionClass GetOptionClass() {
            var optionClass = new OptionClass();
            try {
                var xmlSerializer = new XmlSerializer(typeof(OptionClass));
                using (var stream = File.Open(Path, FileMode.OpenOrCreate)){
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
                DTE.LogError(e.ToString());
                DTE.WriteToOutput(e.ToString());
            }
            return optionClass;

        }

        [XmlArray]
        public BindingList<ConnectionString> ConnectionStrings { get; set; } = new BindingList<ConnectionString>();

        [XmlArray]
        public BindingList<ReferencedAssembliesFolder> ReferencedAssembliesFolders { get; set; } = new BindingList<ReferencedAssembliesFolder>();

        [XmlArray]
        public BindingList<ExceptionsBreak> Exceptions { get; set; } = new BindingList<ExceptionsBreak>();
        [XmlArray]
        public BindingList<ExternalTools> ExternalTools { get; set; } = new BindingList<ExternalTools>();

        [XmlArray]
        public BindingList<ME> MEs { get; set; } = new BindingList<ME>();

        [XmlArray]
        public BindingList<SourceCodeInfo> SourceCodeInfos { get; set; } = new BindingList<SourceCodeInfo>();
        [XmlArray]
        public BindingList<DteCommand> DteCommands { get; set; } = new BindingList<DteCommand>();

        public bool DisableExceptions { get; set; }

        public string DefaultConfiguration => "Debug";

        public void Save() {
            var stringBuilder = new StringBuilder();
            new XmlSerializer(typeof(OptionClass)).Serialize(XmlWriter.Create(stringBuilder), Instance);
            File.WriteAllText(Path, stringBuilder.ToString());
        }


    }

    public class DteCommand:OptionClassBase{
        public string Command{ get; set; }
        public string Shortcut{ get; set; }
    }
    public class ConnectionString : OptionClassBase {
        public string Name { get; set; }
    }

    public class ReferencedAssembliesFolder : OptionClassBase {

        public string Folder { get; set; }
    }

    public class ExternalTools : OptionClassBase{
        public string SolutionRegex{ get; set; }
        public string Path{ get; set; }
        public string Arguments{ get; set; }
        public DTEEvent DTEEvent{ get; set; }
    }

    public enum DTEEvent{
        SolutionOpen,
        SolutionAfterClosing,
        ReferenceRemoved,
        ReferenceChanged,
        ReferenceAdded,
        BuildBegin,
        DocumentSaved
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

        public void AddProjectPaths(){
            if (Directory.Exists(RootPath)) {
                var projectPaths = Directory.GetFiles(RootPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => Regex.IsMatch(Path.GetFileName(s) + "", $@"{ProjectRegex}\z"));
                var paths = projectPaths.Select(path => new ProjectInfo() { Path = path, OutputPath = GetOutPutPath(path) }).Where(info => File.Exists(info.OutputPath)).ToArray();
                ProjectPaths.Clear();
                ProjectPaths.AddRange(paths);
            }
        }
        string GetOutPutPath(string projectPath) {
            try{
                using (var fileStream = File.Open(projectPath, FileMode.Open)) {
                    var streamReader = new StreamReader(fileStream);
                    var readToEnd = streamReader.ReadToEnd();
                    Environment.CurrentDirectory = Path.GetDirectoryName(projectPath) + "";
                    var outPutPath = Path.GetFullPath(GetAttributeValue(readToEnd, "OutputPath"));
                    var assemblyName = GetAttributeValue(readToEnd, "AssemblyName");
                    if (string.IsNullOrEmpty(assemblyName)) {
                        assemblyName = Path.GetFileNameWithoutExtension(projectPath);
                    }
                    return Path.Combine(outPutPath, assemblyName + ".dll");
                }

            }
            catch{
                return null;
            }
        }

        string GetAttributeValue(string readToEnd, string attributeName) {
            var regexObj = new Regex("<" + attributeName + ">([^<]*)</" + attributeName + ">");
            Match matchResults = regexObj.Match(readToEnd);
            return matchResults.Success ? matchResults.Groups[1].Value : null;
        }

    }

    public class ProjectInfo {
        public string Path { get; set; }
        public string OutputPath { get; set; }
    }

    public class OptionClassBase {
    }
}
