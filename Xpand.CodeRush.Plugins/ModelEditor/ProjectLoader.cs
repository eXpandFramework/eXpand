using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.CodeRush.StructuralParser;
using EnvDTE;
using Xpand.CodeRush.Plugins.Extensions;

namespace Xpand.CodeRush.Plugins.ModelEditor {
    public class ProjectLoader {
        private readonly DTE _dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
        bool IsProjectLoaded(string fileName){
            fileName = Path.GetFileNameWithoutExtension(fileName);
            return _dte.Solution.Projects.Cast<Project>().FirstOrDefault(project => project.Name == fileName)!=null;
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

//        public bool Load(IList<AssemblyReference> assemblyReferences ){
//            var sourceCodeInfos = Options.GetDataSource<SourceCodeInfos>();
//            bool[] loadingAttempts=new bool[sourceCodeInfos.Count];
//            for (int i = 0; i < sourceCodeInfos.Count; i++) {
//                _dte.WriteToOutput("SourceCodeInfos:"+sourceCodeInfos[i]);
//                loadingAttempts[i]=LoadProject(sourceCodeInfos[i], assemblyReferences, i);
//            }
//            return loadingAttempts.Any(b => !b);
//        }

//        public bool LoadProject(SourceCodeInfos sourceCodeInfo, IList<AssemblyReference> assemblyReferences, int i1){
//            var projectPath = GetProjectPath(sourceCodeInfo, assemblyReferences, i1);
//            if (projectPath.Key!=null) {
//                if (!IsProjectLoaded(projectPath.Value)){
//                    try{
//                        var fileName = Path.GetFileName(projectPath.Value);
//                        _dte.WriteToOutput("Loading " +fileName);
//                        DevExpress.CodeRush.Core.CodeRush.Solution.Active.AddFromFile(projectPath.Value);
//                        GetSolutionContext(projectPath.Value, context => {
//                            context.ShouldBuild = false;
//                        });
//                        _dte.WriteToOutput(fileName + " loaded!");
//                        return true;
//                    }
//                    catch (Exception e){
//                        _dte.WriteToOutput(e.ToString());
//                        return false;
//                    }
//                }
//            }
//            return false;
//        }

//        private KeyValuePair<AssemblyReference,string> GetProjectPath(SourceCodeInfos sourceCodeInfo, IList<AssemblyReference> assemblyReferences, int i1){
//            string key = String.Format("{0}_{1}", i1, sourceCodeInfo.ProjectRegex);
////            var projectPaths = Options.GetDataSource<SourceCodeInfos>();
////            _dte.WriteToOutput("ProjectsCount:" + projectPaths.Length);
////            return projectPaths.Select(GetProjectPath(assemblyReferences)).FirstOrDefault(pair => pair.Key!=null);
//        }

        private Func<string, KeyValuePair<AssemblyReference, string>> GetProjectPath(IList<AssemblyReference> assemblyReferences){
            return projectPath =>{
                var strings = projectPath.Split('|');
                var assemblyReference = GetAssemblyReference(assemblyReferences, strings[1]);
                return new KeyValuePair<AssemblyReference, string>(assemblyReference, strings[0]);
            };
        }

        AssemblyReference GetAssemblyReference(IEnumerable<AssemblyReference> assemblyReferences, string assemblyPath){
            return assemblyReferences.Where(reference =>{
                var b = reference.FilePath.ToLower() == assemblyPath.ToLower();
                if (!b){
                    b = Path.GetFileName(reference.FilePath.ToLower()) == Path.GetFileName(assemblyPath.ToLower());
                }
                return b && VersionMatches(assemblyPath);
            }).FirstOrDefault();
        }

        private bool VersionMatches(string assemblyPath){
            return Assembly.ReflectionOnlyLoadFrom(assemblyPath).VersionMatch();
        }

        public bool Load(List<AssemblyReference> selectedAssemblyReferences){
            throw new NotImplementedException();
        }
    }
}