using CmdLine;

namespace PDBLinker{
//    [CommandLineArguments(Program = "SimpleCopy", Title = "Simple Copy Title", Description = "Sample copy command")]
    public class Argument {
        [CommandLineParameter(Command = "?", Default = false, Description = "Show Help", Name = "Help", IsHelp = true)]
        public bool Help { get; set; }

        [CommandLineParameter(Name = "sourcedir", ParameterIndex = 1, Required = true, Description = "The source code directory.")]
        public string SourceDir { get; set; }

        [CommandLineParameter(Name = "pdbdir",Required =true, ParameterIndex = 2, Description = "The symbols folder")]
        public string PDBDir { get; set; }

        [CommandLineParameter(Name = "BuildConfiguration", ParameterIndex = 3,Default = "Release")]
        public string BuildConfiguration { get; set; }

        [CommandLineParameter(Name = "DbgToolsPath", ParameterIndex = 4, Default = @"..\..\..\Tool\srcsrv\")]
        public string DbgToolsPath { get; set; }
    }
}