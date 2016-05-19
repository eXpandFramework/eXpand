using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.CodeRush.Diagnostics.General;
using DevExpress.CodeRush.StructuralParser;
using EnvDTE;
using Xpand.CodeRush.Plugins.Extensions;

namespace Xpand.CodeRush.Plugins.ModelEditor {
    public class ProjectLoader {
        private readonly DTE _dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
        bool IsProjectLoaded(string fileName){
            fileName = Path.GetFileNameWithoutExtension(fileName);
            return DevExpress.CodeRush.Core.CodeRush.Solution.Active.Projects.OfType<Project>().FirstOrDefault(project => project.Name == fileName)!=null;
        }

        void GetSolutionContext(string projectFileName, Action<SolutionContext> contextAction) {
            var solutionContexts = _dte.Solution.SolutionBuild.ActiveConfiguration.SolutionContexts;
            for (int i = 0; i < solutionContexts.Count+1; i++) {
                SolutionContext solutionContext = GetSolutionContext(i, solutionContexts);
                if (solutionContext != null && Path.GetFileName(projectFileName) == Path.GetFileName(solutionContext.ProjectName)) {
                    contextAction.Invoke(solutionContext);
                }
            }
        }

        SolutionContext GetSolutionContext(int i, SolutionContexts solutionContexts) {
            SolutionContext solutionContext = null;
            try {
                solutionContext = solutionContexts.Item(i);
            }
            catch{
            }
            return solutionContext;    
        }

        public bool Load(IList<AssemblyReference> assemblyReferences ) {
            var sourceCodeInfos = Options.GetSourceCodeInfos();
            bool[] failLoads=new bool[sourceCodeInfos.Count];
            for (int i = 0; i < sourceCodeInfos.Count; i++) {
                _dte.WriteToOutput("SourceCodeInfo:"+sourceCodeInfos[i]);
                failLoads[i]=LoadProject(sourceCodeInfos[i], assemblyReferences, i);
            }
            return failLoads.Any(b => !b);
        }

        public bool LoadProject(Options.SourceCodeInfo sourceCodeInfo, IList<AssemblyReference> assemblyReferences, int i1){
            var assemblyReference = GetAssemblyReference(sourceCodeInfo, assemblyReferences, i1);
            if (assemblyReference.Key!=null) {
                if (!IsProjectLoaded(assemblyReference.Value)){
                    try{
                        var fileName = Path.GetFileName(assemblyReference.Value);
                        _dte.WriteToOutput("Loading " +fileName);
                        DevExpress.CodeRush.Core.CodeRush.Solution.Active.AddFromFile(assemblyReference.Value);
                        GetSolutionContext(assemblyReference.Value, context => {
                            context.ShouldBuild = false;
                        });
                        _dte.WriteToOutput(fileName + " loaded!");
                        return true;
                    }
                    catch (Exception e){
                        _dte.WriteToOutput(e.ToString());
                        return false;
                    }
                }
            }
            return false;
        }

        private KeyValuePair<AssemblyReference,string> GetAssemblyReference(Options.SourceCodeInfo sourceCodeInfo, IList<AssemblyReference> assemblyReferences, int i1){
            string key = String.Format("{0}_{1}", i1, sourceCodeInfo.ProjectRegex);
            var projectPaths = Options.Storage.ReadStrings(Options.ProjectPaths, key);
            Log.Send("ProjectsCount:" + projectPaths.Length);
            foreach (string projectPath in projectPaths){
                var strings = projectPath.Split('|');
                var assemblyPath = strings[1].ToLower();
                var assemblyReference = GetAssemblyReferenceCore(assemblyReferences, assemblyPath);
                if (assemblyReference != null) {
                    Log.Send("Path found");
                    return new KeyValuePair<AssemblyReference, string>(assemblyReference, strings[0]);
                }
            }
            return new KeyValuePair<AssemblyReference, string>();
        }

        AssemblyReference GetAssemblyReferenceCore(IEnumerable<AssemblyReference> assemblyReferences, string assemblyPath) {
            return assemblyReferences.FirstOrDefault(reference => {
                var b = reference.FilePath.ToLower() == assemblyPath;
                if (!b) {
                    b = Path.GetFileName(reference.FilePath.ToLower()) == Path.GetFileName(assemblyPath);
                }
                return b;
            });
        }
    }
}