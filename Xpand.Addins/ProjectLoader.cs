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


        public void Load(IList<AssemblyReference> assemblyReferences, string constants) {
            Log.Send("References Count:"+assemblyReferences.Count());
            var sourceCodeInfos = Options.GetSourceCodeInfos();
            for (int i = 0; i < sourceCodeInfos.Count; i++) {
                Log.Send("SourceCodeInfo:"+sourceCodeInfos[i]);
                LoadProject(sourceCodeInfos[i], assemblyReferences,i);
            }
            CodeRush.ApplicationObject.Solution.CollapseAllFolders();
        }

        void LoadProject(Options.SourceCodeInfo sourceCodeInfo, IList<AssemblyReference> assemblyReferences, int i1) {
            var readStrings = Options.Storage.ReadStrings(Options.ProjectPaths, i1+"_"+sourceCodeInfo.ProjectRegex);
            Log.Send("ProjectsCount:"+readStrings.Count());
            for (int i = 0; i < readStrings.Count(); i++) {
                var strings = readStrings[i].Split('|');
                var assemblyPath = strings[1].ToLower();
                var assemblyReference = assemblyReferences.Where(reference => reference.FilePath.ToLower() == assemblyPath).FirstOrDefault();
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
            }
        }


    }
}