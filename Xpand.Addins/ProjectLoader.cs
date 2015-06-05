using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.Diagnostics.General;
using DevExpress.CodeRush.StructuralParser;
using EnvDTE;
using XpandAddIns.Extensions;
using Project = EnvDTE.Project;

namespace XpandAddIns {
    public class ProjectLoader {
        bool IsProjectLoaded(string fileName) {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            return CodeRush.Solution.Active.Projects.OfType<Project>().FirstOrDefault(project => project.Name == fileName)!=null;
        }

        void GetSolutionContext(string projectFileName, Action<SolutionContext> contextAction) {
            SolutionContexts solutionContexts = CodeRush.Solution.Active.DTE.Solution.SolutionBuild.ActiveConfiguration.SolutionContexts;
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


        public void Load(IList<AssemblyReference> assemblyReferences,Action<string> notFound ) {
            Log.Send("References Count:"+assemblyReferences.Count());
            var sourceCodeInfos = Options.GetSourceCodeInfos();
            for (int i = 0; i < sourceCodeInfos.Count; i++) {
                Log.Send("SourceCodeInfo:"+sourceCodeInfos[i]);
                LoadProject(sourceCodeInfos[i], assemblyReferences,i,notFound);
            }
            CodeRush.ApplicationObject.Solution.CollapseAllFolders();
        }

        public void LoadProject(Options.SourceCodeInfo sourceCodeInfo, IList<AssemblyReference> assemblyReferences, int i1,Action<string> notFound ) {
            string key = String.Format("{0}_{1}", i1, sourceCodeInfo.ProjectRegex);
            var readStrings = Options.Storage.ReadStrings(Options.ProjectPaths, key);
            Log.Send("ProjectsCount:"+readStrings.Count());
            for (int i = 0; i < readStrings.Count(); i++) {
                var strings = readStrings[i].Split('|');
                var assemblyPath = strings[1].ToLower();
                var assemblyReference = GetAssemblyReference(assemblyReferences, assemblyPath);
                if (assemblyReference != null) {
                    Log.Send("Path found");
                    if (!IsProjectLoaded(strings[0])) {
                        try {
                            CodeRush.Solution.Active.AddFromFile(strings[0]);
                            GetSolutionContext(strings[0], context => {
                                context.ShouldBuild = false;
                            });
                        }
                        catch (Exception e) {
                            Log.SendException(e);
                        }
                    }
                }
                else {
                    notFound.Invoke(assemblyPath);
                }
            }
        }

        AssemblyReference GetAssemblyReference(IEnumerable<AssemblyReference> assemblyReferences, string assemblyPath) {
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