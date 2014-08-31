using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using CmdLine;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.Build.Evaluation;

namespace PDBLinker {
    class Program {
        private static ILog _logger;

        public static void Main(string[] args){
            _logger=GetLogger();
            try {
                var argument = CommandLine.Parse<Argument>();
                var projectFiles = new List<string>();
                projectFiles.AddRange(Directory.GetFiles(argument.SourceDir, "*.csproj", SearchOption.AllDirectories));
                projectFiles.AddRange(Directory.GetFiles(argument.SourceDir, "*.vbproj", SearchOption.AllDirectories));
                var failedProjects = new List<KeyValuePair<string,string>>();
                var projects = projectFiles.Select(s => new Project(s)).ToList();
                _logger.Info(string.Format("Found '{0}' project(s)", projectFiles.Count));
                var pdbFiles = Directory.GetFiles(argument.PDBDir,"*.pdb").ToArray();
                _logger.Info(string.Format("Found '{0}' pdb files", pdbFiles.Length));
                foreach (var pdbFile in pdbFiles){
                    var currentProject = projects.FirstOrDefault(project => Path.GetFileName(project.GetOutputPdbFile()) == Path.GetFileName(pdbFile));
                    if (currentProject!=null){
                        var pdbStoreManager = new PdbStoreManager(argument.DbgToolsPath);
                        var srcSrvSection = CreatePdbSrcSrvSection(currentProject, pdbStoreManager, pdbFile);
                        var writeSrcSrv = pdbStoreManager.WriteSrcSrv(pdbFile, srcSrvSection);
                        var fileName = Path.GetFileNameWithoutExtension(pdbFile);
                        if (!string.IsNullOrEmpty(writeSrcSrv)){
                            failedProjects.Add(new KeyValuePair<string,string>(pdbFile,writeSrcSrv));
                            _logger.Error(fileName+"-->"+writeSrcSrv);
                        }
                        else{
                            _logger.Info(fileName+" indexed successfully!");
                        }
                    }
                    else{
                        failedProjects.Add(new KeyValuePair<string, string>(pdbFile,"Project not found"));
                        _logger.Error(pdbFile +"-->Project not found");
                    }
                }
            }
            catch (CommandLineException exception) {
                _logger.Error(exception.ArgumentHelp.Message);
                _logger.Error(exception.ArgumentHelp.GetHelpText(Console.BufferWidth));
            }
            if (Debugger.IsAttached)
                Console.ReadKey();
        }

        private static PdbSrcSrvSection CreatePdbSrcSrvSection(Project currentProject, PdbStoreManager pdbStoreManager,
            string pdbFile){
            var srcSrvSection = new PdbSrcSrvSection();
            srcSrvSection.Ini.Add("VERSION", "2");
            srcSrvSection.Ini.Add("INDEXVERSION", "2");
            srcSrvSection.Ini.Add("DATETIME", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            srcSrvSection.Variables.Add("SRCSRVTRG", "%var2%");
            var sourceList = GetSourceList(currentProject, pdbStoreManager, pdbFile);
            foreach (var source in sourceList){
                srcSrvSection.Sources.Add(new[]{source});
            }
            return srcSrvSection;
        }

        private static IEnumerable<string> GetSourceList(Project currentProject, PdbStoreManager pdbStoreManager, string pdbFile){
            var compilableItems = currentProject.GetCompilableItems();
            var indexedFiles = pdbStoreManager.GetIndexedFiles(pdbFile);
            if (indexedFiles.Any()){
                var longestDir = Enumerable.Range(0, indexedFiles.Min(s => s.Length)).Reverse()
                .Select(len => new { len, possibleMatch = indexedFiles.First().Substring(0, len) })
                .Where(@t => indexedFiles.All(f => f.StartsWith(@t.possibleMatch)))
                .Select(@t => @t.possibleMatch).First();
                return compilableItems.Select(compilableItem => new { compilableItem, targetPath = Path.Combine(currentProject.DirectoryPath, compilableItem.EvaluatedInclude) })
                        .Select(@t => Path.Combine(longestDir, @t.compilableItem.EvaluatedInclude) + "*" + @t.targetPath);
            }
            return Enumerable.Empty<string>();
        }

        private static ILog GetLogger(){
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            var consoleAppender = new ConsoleAppender();
            hierarchy.Root.AddAppender(consoleAppender);
            var patternLayout = new PatternLayout { ConversionPattern = "%m%n" };
            consoleAppender.Layout=patternLayout;
            hierarchy.Configured = true;
            return LogManager.GetLogger(typeof (Program));
        }
    }
}
