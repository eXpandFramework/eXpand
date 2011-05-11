using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.StructuralParser;
using EnvDTE;
using XpandAddIns.Extensioons;
using Project = EnvDTE.Project;
using Xpand.Utils.Helpers;

namespace XpandAddIns {
    public class ProjectLoader {
        bool IsProjectLoaded(string fileName) {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            return CodeRush.Solution.Active.Projects.OfType<Project>().FirstOrDefault(project => project.Name == fileName)!=null;
        }

        void GetSolutionContext(string projectFileName, Action<SolutionContext> contextAction) {
            SolutionContexts solutionContexts = CodeRush.Solution.Active.DTE.Solution.SolutionBuild.ActiveConfiguration.SolutionContexts;
            for (int i = 1; i < solutionContexts.Count-1; i++) {
                SolutionContext solutionContext = GetSolutionContext(i, solutionContexts);
                if (solutionContext != null && projectFileName == Path.GetFileName(solutionContext.ProjectName)) {
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


        public void Load(IEnumerable<AssemblyReference> assemblyReferences) {
            assemblyReferences = GetSelectedAssemblyReferences(assemblyReferences);
            Options.GetSourceCodeInfos().Each(info => LoadProject(info,assemblyReferences));
            CodeRush.ApplicationObject.Solution.CollapseAllFolders();
        }

        void LoadProject(Options.SourceCodeInfo sourceCodeInfo, IEnumerable<AssemblyReference> assemblyReferences) {
            var readStrings = Options.Storage.ReadStrings(Options.ProjectPaths, sourceCodeInfo.ProjectRegex);
            for (int i = 0; i < readStrings.Count(); i++) {
                var strings = readStrings[i].Split('|');
                var assemblyPath = strings[1].ToLower();
                var assemblyReference = assemblyReferences.Where(reference => reference.FilePath.ToLower() == assemblyPath).FirstOrDefault();
                if (assemblyReference != null) {
                    if (!IsProjectLoaded(strings[0])) {
                        CodeRush.Solution.Active.AddFromFile(strings[0]);
                        GetSolutionContext(strings[0], context => {
                            context.ShouldBuild = false;
                        });
                    }
                }
            }
        }

        IEnumerable<AssemblyReference> GetSelectedAssemblyReferences(IEnumerable<AssemblyReference> assemblyReferences) {
            DTE dte = CodeRush.ApplicationObject;
            var items = ((UIHierarchy)dte.Windows.Item(Constants.vsext_wk_SProjectWindow).Object).SelectedItems;
            var selectedItems = ((System.Collections.IEnumerable)items).OfType<UIHierarchyItem>().Select(item => item.Name);
            assemblyReferences = assemblyReferences.Where(reference => selectedItems.Contains(reference.Name));
            return assemblyReferences;
        }

    }
}